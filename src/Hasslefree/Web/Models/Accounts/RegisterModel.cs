using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Web.Models.Accounts
{
	public class RegisterModel
    {
        public string Email { get; set; }
		public string Title { get; set; }
		public string IdNumber { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public Gender Gender { get; set; }
	}
}
