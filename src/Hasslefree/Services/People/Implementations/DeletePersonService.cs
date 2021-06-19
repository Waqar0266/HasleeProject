using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Data;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.People.Warnings;

namespace Hasslefree.Services.People.Implementations
{
	public class DeletePersonService : IDeletePersonService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Person> PersonRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private List<Person> _people;

		#endregion

		#region Constructor

		public DeletePersonService
		(
			IDataRepository<Person> personRepo,
			IDataContext database
		)
		{
			// Repos
			PersonRepo = personRepo;

			// Other
			Database = database;
		}

		#endregion

		#region IDeletePersonService

		public bool HasWarnings => Warnings.Any();

		public List<PersonWarning> Warnings { get; } = new List<PersonWarning>();

		public IDeletePersonService this[int personId]
		{
			get
			{
				if (personId <= 0) return this;

				var person = PersonRepo.GetById(personId);

				if (person == null) return this;

				_people = _people ?? new List<Person>();

				_people.Add(person);

				return this;
			}
		}

		public IDeletePersonService this[List<int> personIds]
		{
			get
			{
				if (!(personIds?.Any() ?? false)) return this;

				_people = PersonRepo.Table
					.Where(p => personIds.Contains(p.PersonId))
					.ToList();

				return this;
			}
		}

		public bool Remove(bool saveChanges = true)
		{
			if (!IsValid()) return false;

			PersonRepo.Remove(_people);

			if (saveChanges) Database.SaveChanges();

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (!(_people?.Any() ?? false))
				Warnings.Add(new PersonWarning(PersonWarningCode.PersonsNotFound));

			return !HasWarnings;
		}

		#endregion
	}
}
