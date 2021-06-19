using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Hasslefree.Data.Extensions
{
	public static class EfQueryUtils
	{
		public static string ToWrappedString(this ObjectQuery query, out ObjectParameterCollection parameters)
		{
			var trace = query.ToTraceString();
			parameters = query.Parameters;
			var positions = query.GetPropertyPositions();

			// the query should be SELECT\n
			//  Column AS NNN
			//  FROM
			// so we regex this out
			var regex = new Regex("^SELECT(?<columns>.*?)FROM", RegexOptions.Multiline);
			var result = regex.Match(trace.Replace(Environment.NewLine, ""));
			var cols = result.Groups["columns"];

			// then we have the columns so split to get each
			const string As = " AS ";
			var colNames = cols.Value.Split(new string[] { "`, " }, StringSplitOptions.None).Select(a => a.Contains(As) ? a.Substring(a.IndexOf(As, StringComparison.InvariantCulture) + As.Length) : a.Contains("`.`") ? $"`{a.Split(new[] { "`.`" }, StringSplitOptions.None)[1]}" : a).ToArray();
			colNames = colNames.Select(a => a.TrimEnd('`') + '`').ToArray();

			var wrapped = "SELECT " + String.Join(Environment.NewLine + ", ", positions.Select(a => $"{colNames[a.Value]}{As} `{a.Key}`")) + " FROM (" + trace + ") WrappedQuery ";

			// Replace all @gp{number} items with strings
			wrapped = Regex.Replace(wrapped, "@gp[0-9]+", "''");

			return wrapped;
		}

		public static Dictionary<String, Int32> GetPropertyPositions(this ObjectQuery query)
		{
			// get private ObjectQueryState ObjectQuery._state;
			// of actual type internal class
			//      System.Data.Objects.ELinq.ELinqQueryState
			object queryState = GetProperty(query, "QueryState");
			AssertNonNullAndOfType(queryState, "System.Data.Entity.Core.Objects.ELinq.ELinqQueryState");

			// get protected ObjectQueryExecutionPlan ObjectQueryState._cachedPlan;
			// of actual type internal sealed class
			//      System.Data.Objects.Internal.ObjectQueryExecutionPlan
			object plan = GetField(queryState, "_cachedPlan");
			AssertNonNullAndOfType(plan, "System.Data.Entity.Core.Objects.Internal.ObjectQueryExecutionPlan");

			// get internal readonly DbCommandDefinition ObjectQueryExecutionPlan.CommandDefinition;
			// of actual type internal sealed class
			//      System.Data.EntityClient.EntityCommandDefinition
			object commandDefinition = GetField(plan, "CommandDefinition");
			AssertNonNullAndOfType(commandDefinition, "System.Data.Entity.Core.EntityClient.Internal.EntityCommandDefinition");

			// get private readonly IColumnMapGenerator EntityCommandDefinition._columnMapGenerator;
			// of actual type private sealed class
			//      System.Data.EntityClient.EntityCommandDefinition.ConstantColumnMapGenerator
			var columnMapGeneratorArray = GetField(commandDefinition, "_columnMapGenerators") as object[];
			AssertNonNullAndOfType(columnMapGeneratorArray, "System.Data.Entity.Core.EntityClient.Internal.EntityCommandDefinition+IColumnMapGenerator[]");

			var columnMapGenerator = columnMapGeneratorArray[0];

			// get private readonly ColumnMap ConstantColumnMapGenerator._columnMap;
			// of actual type internal class
			//      System.Data.Query.InternalTrees.SimpleCollectionColumnMap
			object columnMap = GetField(columnMapGenerator, "_columnMap");
			AssertNonNullAndOfType(columnMap, "System.Data.Entity.Core.Query.InternalTrees.SimpleCollectionColumnMap");

			// get internal ColumnMap CollectionColumnMap.Element;
			// of actual type internal class
			//      System.Data.Query.InternalTrees.RecordColumnMap
			object columnMapElement = GetProperty(columnMap, "Element");
			AssertNonNullAndOfType(columnMapElement, "System.Data.Entity.Core.Query.InternalTrees.RecordColumnMap");

			// get internal ColumnMap[] StructuredColumnMap.Properties;
			// array of internal abstract class
			//      System.Data.Query.InternalTrees.ColumnMap
			Array columnMapProperties = GetProperty(columnMapElement, "Properties") as Array;
			AssertNonNullAndOfType(columnMapProperties, "System.Data.Entity.Core.Query.InternalTrees.ColumnMap[]");

			int n = columnMapProperties.Length;
			var propertyPositions = new Dictionary<String, Int32>();
			for (int i = 0; i < n; i++)
			{
				// get value at index i in array
				// of actual type internal class
				//      System.Data.Query.InternalTrees.ScalarColumnMap
				object column = columnMapProperties.GetValue(i);
				AssertNonNullAndOfType(column, "System.Data.Entity.Core.Query.InternalTrees.ScalarColumnMap");

				string colName = (string)GetProperty(column, "Name");
				// can be used for more advanced bingings

				// get internal int ScalarColumnMap.ColumnPos;
				object columnPositionOfAProperty = GetProperty(column, "ColumnPos");
				AssertNonNullAndOfType(columnPositionOfAProperty, "System.Int32");

				propertyPositions.Add(colName, (Int32)columnPositionOfAProperty);
			}
			return propertyPositions;
		}

		static object GetProperty(object obj, string propName)
		{
			PropertyInfo prop = obj.GetType().GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (prop == null) throw EFChangedException();
			return prop.GetValue(obj, new object[0]);
		}

		static object GetField(object obj, string fieldName)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null) throw EFChangedException();
			return field.GetValue(obj);
		}

		static void AssertNonNullAndOfType(object obj, string fullName)
		{
			if (obj == null) throw EFChangedException();
			string typeFullName = obj.GetType().FullName;
			if (typeFullName != fullName) throw EFChangedException();
		}

		static InvalidOperationException EFChangedException()
		{
			return new InvalidOperationException("Entity Framework internals has changed, please review and fix reflection code");
		}
	}
}
