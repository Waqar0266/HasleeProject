using Hasslefree.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using Hasslefree.Data.Extensions;
using Hasslefree.Data.Models;

namespace Hasslefree.Data
{
	public class DataRepository<T> : IDataRepository<T> where T : BaseEntity
	{
		#region Fields

		private IDataContext Db { get; }
		private DbSet<T> Set { get; set; }

		#endregion

		#region Constructor
		/// <summary>
		/// Constructs a new instance of a data repository
		/// </summary>
		/// <param name="context"></param>
		public DataRepository(IDataContext context)
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
		/// Insert a new entity into the repository
		/// </summary>
		/// <param name="entity"></param>
		public void Insert(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			Entities.Add(entity);
			Db.SaveChanges();
		}

		/// <summary>
		/// Inserts a new entity
		/// </summary>
		/// <param name="entities"></param>
		public void Insert(IEnumerable<T> entities)
		{
			if(entities == null) throw new ArgumentNullException(nameof(entities));
			var baseEntities = entities.ToList();
			if(!baseEntities.Any()) throw new ArgumentNullException(nameof(entities));
			if(baseEntities.Any(e => e == null)) throw new ArgumentNullException(nameof(entities));

			Entities.AddRange(baseEntities);
			Db.SaveChanges();
		}

		/// <summary>
		/// Update an existing entity in the repository
		/// </summary>
		/// <param name="entity"></param>
		public void Update(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			//Look for the entity in the object context (if is already exists)
			var e = FindEntity(entity);

			//The entity is not in the object context, attach it
			if (e == null)
			{
				Entities.Attach(entity);
				Throw(Db).Entry(entity).State = EntityState.Modified;
			}

			//The entity is already in the object context, set the values for it
			else
			{
				Throw(Db).Entry(e).CurrentValues.SetValues(entity);
			}

			//Save to the database
			Db.SaveChanges();
		}

		/// <summary>
		/// Update an existing entity in the repository
		/// </summary>
		/// <param name="entity"></param>
		public void Edit(T entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			//Look for the entity in the object context (if is already exists)
			var e = FindEntity(entity);

			//The entity is not in the object context, attach it
			if(e == null)
			{
				Entities.Attach(entity);
				Throw(Db).Entry(entity).State = EntityState.Modified;
			}

			//The entity is already in the object context, set the values for it
			else
			{
				Throw(Db).Entry(e).CurrentValues.SetValues(entity);
			}
		}

		/// <summary>
		/// Deletes an existing entity from the repository
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			//Look for the entity in the object context (if is already exists)
			var e = FindEntity(entity);

			//The entity is not in the object context, attach it
			if (e == null)
			{
				Entities.Attach(entity);
				Throw(Db).Entry(entity).State = EntityState.Deleted;
			}

			//Delete the entity
			Entities.Remove(entity);
			Db.SaveChanges();
		}
		
		/// <summary>
		/// Adds the entity to the object context
		/// </summary>
		/// <param name="entity"></param>
		public void Add(T entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			Entities.Add(entity);
		}

		/// <summary>
		/// Adds entities to the object context
		/// </summary>
		/// <param name="entities"></param>
		public void Add(IEnumerable<T> entities)
		{
			if(entities == null) throw new ArgumentNullException(nameof(entities));
			var baseEntities = entities.ToList();
			if(baseEntities.Count == 0) throw new ArgumentNullException(nameof(entities));
			if(baseEntities.Any(e => e == null)) throw new ArgumentNullException(nameof(entities));

			Entities.AddRange(baseEntities);
		}

		/// <summary>
		/// Attach the entity to the object context
		/// </summary>
		/// <param name="entity"></param>
		public void Attach(T entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			//Look for the entity in the object context (if is already exists)
			var e = FindEntity(entity);

			//The entity is not in the object context, attach it
			if(e == null)
			{
				Entities.Attach(entity);
				Throw(Db).Entry(entity).State = EntityState.Modified;
			}
		}

		/// <summary>
		/// Set the item as deleted in the object context
		/// </summary>
		/// <param name="entity"></param>
		public void Remove(T entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			//Look for the entity in the object context (if is already exists)
			var e = FindEntity(entity);

			//The entity is not in the object context, attach it
			if(e == null) Entities.Attach(entity);

			//Delete the entity
			Entities.Remove(entity);
		}

		/// <summary>
		/// Set the item as deleted in the object context
		/// </summary>
		/// <param name="entities"></param>
		public void Remove(IEnumerable<T> entities)
		{
			if(entities == null) throw new ArgumentNullException(nameof(entities));
			var baseEntities = entities.ToList();
			if(!baseEntities.Any()) throw new ArgumentNullException(nameof(entities));
			if(baseEntities.Any(e => e == null)) throw new ArgumentNullException(nameof(entities));

			foreach(var entity in baseEntities)
			{ 
				//Look for the entity in the object context (if is already exists)
				var e = FindEntity(entity);

				//The entity is not in the object context, attach it
				if(e == null) Entities.Attach(entity);
			}

			//Delete the entity
			Entities.RemoveRange(baseEntities);
		}

		public void SelectInto<TQ>(IQueryable<TQ> query)
		{
			var type = typeof(T).Name;

			var oQuery = GetObjectQuery(query);
			var wrappedSql = oQuery.ToWrappedString(out var @params);

			var columns = typeof(TQ).GetProperties().Select(s => s.Name).ToList();

			var parameters = @params.Select(p => new
			{
				Name = p.Name,
				Value = p.Value?.ToString()
			}).ToDictionary(a => a.Name, b => b.Value);

			var insertQ = $"INSERT INTO `{type}`({String.Join(", ", columns.Select(s => $"`{s}`"))})\n{wrappedSql};";


			Db.Execute(insertQ, parameters, 600);
		}
		
		private ObjectQuery<TQ> GetObjectQuery<TQ>(IQueryable<TQ> query)
		{
			return (ObjectQuery<TQ>)GetPropertyValue(GetPropertyValue(query, "InternalQuery"), "ObjectQuery");
		}

		private Object GetPropertyValue(Object o, String name)
		{
			return o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).First(x => x.Name == name).GetValue(o, null);
		}

		/// <summary>
		/// Access the entire entity table
		/// </summary>
		public IQueryable<T> Table => Entities;

		/// <summary>
		/// Access the entire set of entities
		/// </summary>
		protected DbSet<T> Entities => Set ?? (Set = Db.Set<T>());

		/// <summary>
		/// Throw the interface to a database context
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private static DbContext Throw(IDataContext context) => context as DbContext;

		/// <summary>
		/// Gets the primary key values form the entity
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private T FindEntity(T entity)
		{
			var primaryKeys = Db.GetPrimaryKey(entity);

			if(primaryKeys.Values.Count == 1)
				return Entities.Find(primaryKeys.Values.FirstOrDefault());

			return Entities.Find(primaryKeys.Values.ToArray());
		}

		/// <summary>
		/// Bulk insert entities
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="bulkOperationFactory"></param>
		public void BulkInsert(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null)
		{
			Db.BulkInsert(entities, bulkOperationFactory);
		}

		/// <summary>
		/// Bulk update entities
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="bulkOperationFactory"></param>
		public void BulkUpdate(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null)
		{
			Db.BulkUpdate(entities, bulkOperationFactory);
		}

		/// <summary>
		/// Bulk merge entities
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="bulkOperationFactory"></param>
		public void BulkMerge(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null)
		{
			Db.BulkMerge(entities, bulkOperationFactory);
		}

		/// <summary>
		/// Bulk delete entities
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="bulkOperationFactory"></param>
		public void BulkDelete(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null)
		{
			Db.BulkDelete(entities, bulkOperationFactory);
		}
	}
}
