using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hasslefree.Core.Domain;
using Hasslefree.Data.Models;

namespace Hasslefree.Data
{
	public interface IAsynchronousDataContext
	{
		Task<int> SaveChangesAsync();
		Task BulkSaveChangesAsync();	
		Task ExecuteSqlAsync(String sql);
		Task<int> ExecuteAsync(string sql, Dictionary<string, string> parameters, Int32 timeout = 30);
		Task BulkInsertAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		Task BulkUpdateAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		Task BulkMergeAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
		Task BulkDeleteAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity;
	}
}