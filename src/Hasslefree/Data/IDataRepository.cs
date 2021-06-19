using Hasslefree.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Data.Models;

namespace Hasslefree.Data
{
	public interface IDataRepository<T> where T : BaseEntity
	{
		T GetById(object id);
		void Insert(T entity);
		void Insert(IEnumerable<T> entity);
		void Update(T entity);
		void Delete(T entity);
		void Add(T entity);
		void Add(IEnumerable<T> entity);
		void Remove(T entity);
		void Remove(IEnumerable<T> entity);
		void Attach(T entity);
		void Edit(T entity);
		IQueryable<T> Table { get; }
		void SelectInto<TQ>(IQueryable<TQ> query);
		void BulkInsert(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null);
		void BulkUpdate(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null);
		void BulkMerge(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null);
		void BulkDelete(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null);
	}
}