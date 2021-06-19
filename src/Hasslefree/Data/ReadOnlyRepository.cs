using Hasslefree.Core.Domain;
using System.Data.Entity;
using System.Linq;

namespace Hasslefree.Data
{
	public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : BaseEntity
	{
		#region Fields

		private IReadOnlyContext Db { get; }
		private DbSet<T> Set { get; set; }
		private bool _skipAsNoTracking = false;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of a data repository
		/// </summary>
		/// <param name="context"></param>
		public ReadOnlyRepository(IReadOnlyContext context)
		{
			Db = context;
		}

		#endregion

		/// <summary>
		/// Fetch an entity from the repository by id
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>Returns an entity or null"/></returns>
		public T GetById(object id) => Entities.Find(id);

		/// <summary>
		/// Access the entire entity table
		/// </summary>
		public IQueryable<T> Table => _skipAsNoTracking ? Entities : Entities.AsNoTracking();

		public IReadOnlyRepository<T> WithTracking()
		{
			_skipAsNoTracking = true;
			return this;
		}

		/// <summary>
		/// Access the entire set of entities
		/// </summary>
		protected DbSet<T> Entities => Set ?? (Set = Db.Set<T>());
	}
}
