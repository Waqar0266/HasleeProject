using Hasslefree.Core.Domain;
using System.Linq;

namespace Hasslefree.Data
{
	public interface IReadOnlyRepository<T> where T : BaseEntity
	{
		T GetById(object id);
		IQueryable<T> Table { get; }
		IReadOnlyRepository<T> WithTracking();
	}
}