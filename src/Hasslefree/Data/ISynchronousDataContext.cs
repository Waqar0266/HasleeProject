using System;
using System.Collections.Generic;
using Hasslefree.Core.Domain;
using Hasslefree.Data.Models;

namespace Hasslefree.Data
{
	public interface ISynchronousDataContext
	{
		int SaveChanges();
		void BulkSaveChanges();	
		void ExecuteSql(String sql);
		int Execute(string sql, Dictionary<string, string> parameters, Int32 timeout = 30);
		void BulkInsert<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		void BulkUpdate<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		void BulkMerge<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		void BulkDelete<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
	}
}