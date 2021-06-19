using System;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Web.Models.Login
{
	public class LoginPerson
	{
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string MiddleNames { get; set; }
		public string Surname { get; set; }
		public DateTime? Birthday { get; set; }
		public string Alias { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public Gender Gender { get; set; }
	}
}
