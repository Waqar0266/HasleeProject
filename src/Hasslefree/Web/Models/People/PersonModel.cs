using System;

namespace Hasslefree.Web.Models.People
{
	public class PersonModel
	{
		public Int32 PersonId { get; set; }
		public String PersonTitle { get; set; }
		public String Mobile { get; set; }
		public DateTime? Birthday { get; set; }
		public String FirstName { get; set; }
		public String Surname { get; set; }
		public String Email { get; set; }
		public String Gender { get; set; }
	}
}
