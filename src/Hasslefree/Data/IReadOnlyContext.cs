using System;
using Hasslefree.Core.Domain;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;

namespace Hasslefree.Data
{
	public interface IReadOnlyContext
	{
		DbSet<T> Set<T>() where T : BaseEntity;
		bool DatabaseExists();
		IEnumerable<T> Query<T>(string sql, object param = null, Boolean dapper = false);
		DbConnection Connection { get; }
		string ProviderName { get; }
	}
}