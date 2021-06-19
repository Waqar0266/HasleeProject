using Hasslefree.Core.Domain;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Data.Models
{
	public class BulkOperationOptions<TEntity> where TEntity : BaseEntity
	{
		public Boolean IncludeGraph { get; set; } = false;

		/// <summary>
		/// As new data is inserted read the output (primary keys will be returned etc)
		/// <para></para>
		/// By default true - save time on inserts/merges when set to false
		/// </summary>
		public bool AutoMapOutputDirection { get; set; } = true;

		/// <summary>
		/// Set the primary key for this bulk operation (not required)
		/// </summary>
		public Expression<Func<TEntity, object>> ColumnPrimaryKeyExpression { get; set; } = null;

		/// <summary>
		/// Set which columns will be inserted/updated (not required)
		/// </summary>
		public Expression<Func<TEntity, object>> ColumnInputExpression { get; set; } = null;

		/// <summary>
		/// When doing a merge and the entry exists, ignore the following columns (not required)
		/// </summary>
		public Expression<Func<TEntity, object>> IgnoreOnMergeUpdateExpression { get; set; } = null;
	}
}
