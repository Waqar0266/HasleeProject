using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Reflection;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class ModelExtensions
	{
		/// <summary>
		/// Creates a new copy in memory of the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Clone<T>(this T obj) where T : class
		{
			var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<T, T>());
			var mapper = mapConfig.CreateMapper();

			var newObj = mapper.Map<T>(obj);

			return newObj;
		}

		/// <summary>
		/// Creates a new copy in memory of the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="oList"></param>
		/// <returns></returns>
		public static List<T> CloneList<T>(this List<T> oList) where T : class
		{
			var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<T, T>().ReverseMap());

			var mapper = mapConfig.CreateMapper();

			var list = new List<T>();
			foreach (var pModel in oList)
				list.Add(mapper.Map<T>(pModel));

			return list;
		}

		/// <summary>
		/// Directly cast all values from T1 to T2
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="pModel"></param>
		/// <returns></returns>
		public static T2 MapModel<T1, T2>(this T1 pModel) where T1 : class
														  where T2 : class
		{
			var ignoreProps = new List<Type>
			{
				typeof(String),
				typeof(Decimal),
				typeof(Int32),
				typeof(Double),
				typeof(Single),
				typeof(Int16),
				typeof(Boolean)
			};

			var mapConfig = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<T1, T2>();

				var t1Classes = typeof(T1).GetProperties().Where(a => !ignoreProps.Contains(a.PropertyType)).ToList();
				var t2Classes = typeof(T2).GetProperties().Where(a => !ignoreProps.Contains(a.PropertyType)).ToList();

				var mapClasses = (from c1 in t1Classes
								  join c2 in t2Classes on c1.Name equals c2.Name
								  select new
								  {
									  c1,
									  c2
								  });
				foreach (var mapClass in mapClasses)
				{
					var c1Type = mapClass.c1.PropertyType;
					if (c1Type.IsGenericType && c1Type.GetGenericTypeDefinition() == typeof(List<>))
					{
						cfg.CreateMap(mapClass.c1.PropertyType.GetGenericArguments()[0], mapClass.c2.PropertyType.GetGenericArguments()[0]);
					}
					else
						cfg.CreateMap(mapClass.c1.PropertyType, mapClass.c2.PropertyType);
				}
			});
			var mapper = mapConfig.CreateMapper();

			return mapper.Map<T2>(pModel);
		}

		/// <summary>
		/// Directly cast all values from T1 to T2 for a list
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="pModels"></param>
		/// <returns></returns>
		public static List<T2> MapModels<T1, T2>(this List<T1> pModels) where T1 : class
																		where T2 : class
		{
			var mapConfig = new MapperConfiguration(cfg => cfg.CreateMap<T1, T2>());
			var mapper = mapConfig.CreateMapper();

			var list = new List<T2>();
			foreach (var pModel in pModels)
				list.Add(mapper.Map<T2>(pModel));

			return list;
		}

		public static TTarget MapModelProperties<TTarget, TSource>(this TTarget target, TSource source) where TTarget : class
																										where TSource : class
		{
			// Map target into the source, where the source property is null
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<TTarget, TSource>()
					.ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null));
			});
			var mapper = config.CreateMapper();
			mapper.Map(target, source);


			// Map the source into the target to apply the changes
			var rConfig = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TTarget>());
			var rMapper = rConfig.CreateMapper();
			rMapper.Map(source, target);

			return target;
		}

		public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
		{
			IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

			return sequences.Aggregate
			(
				emptyProduct,
				(accumulator, sequence) => accumulator.SelectMany
				(
					accseq => sequence,
					(accseq, item) => accseq.Concat(new[] { item })
				)
			);
		}


		/// <summary>
		/// Allows any list to be changed to a selectlist used by @Html.DropDown
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TProperty1"></typeparam>
		/// <typeparam name="TProperty2"></typeparam>
		/// <param name="list"></param>
		/// <param name="text"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SelectList ToSelectList<TEntity, TProperty1, TProperty2>(this List<TEntity> list, Expression<Func<TEntity, TProperty1>> text, Expression<Func<TEntity, TProperty2>> value)
		{
			// Return if no product or standard prices
			if (list == null) return new SelectList(new object[0].ToList());
			if (list.Count == 0) return new SelectList(new object[0].ToList());

			// Get the selected property
			if (!(text.Body is MemberExpression keySelector)) throw new ArgumentException(nameof(text));
			if (!(value.Body is MemberExpression valueSelector)) throw new ArgumentException(nameof(value));

			var keyProp = keySelector.Member as PropertyInfo;
			if (keyProp == null) throw new ArgumentException(nameof(text));

			var valueProp = valueSelector.Member as PropertyInfo;
			if (valueProp == null) throw new ArgumentException(nameof(value));

			var selectList = new List<(String Key, String Value)>();
			foreach (var item in list)
			{
				selectList.Add((keyProp.GetValue(item)?.ToString(), valueProp.GetValue(item)?.ToString()));
			}

			return new SelectList(selectList.Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).ToList(), "Value", "Text");
		}
	}
}
