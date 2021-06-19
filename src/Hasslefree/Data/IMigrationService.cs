using System.Data.Entity;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Data
{
	public interface IMigrationService : IInstancePerRequest
	{
		void Migrate(DbContext context);
	}
}
