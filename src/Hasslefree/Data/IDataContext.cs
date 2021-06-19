using Hasslefree.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace Hasslefree.Data
{

	public interface IDataContext : ISynchronousDataContext, IAsynchronousDataContext
	{
		DbSet<T> Set<T>() where T : BaseEntity;		
		bool DatabaseExists();	
		IEnumerable<T> Query<T>(String sql, object param = null, Boolean dapper = false);
		DbConnection Connection { get; }
		String ProviderName { get; }
		DbContextTransaction BeginTransaction();
		DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
		Dictionary<String, Object> GetPrimaryKey<T>(T entity) where T : BaseEntity;
	}
}