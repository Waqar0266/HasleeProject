using System;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Web.Models.Accounts.Crud
{
	public class PersonalDetailsCrudModel
	{
		public int PersonId { get; set; }
		public Titles Title { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Gender Gender { get; set; }
		public DateTime? Birthday { get; set; }
	}
}
