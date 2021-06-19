namespace Hasslefree.Core.Infrastructure.Email.Models
{
	public class RegistrationWelcomeModel
	{
		public string StoreLogoUrl { get; set; }
		public bool HasLogo { get; set; }
		public string StoreUrl { get; set; }
		public string StoreName { get; set; }
		public string StorePhone { get; set; }
		public string Email { get; set; }
		public string ResetUrl { get; set; }
		public string FirstName { get; set; }
		public bool ShowAddress { get; set; }
		public string Address { get; set; }
	}
}
