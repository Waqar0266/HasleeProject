using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Hasslefree.Web.Models.Common;
using System;

namespace Hasslefree.Web.Models.Security.AuthorizedUsers
{
	[Validator(typeof(AuthorizedUserBaseValidator<AuthorizedUserBaseModel>))]
	public class AuthorizedUserBaseModel
	{
		public AuthorizedUserBaseModel()
		{
			Action = CrudAction.None;
		}

		public int LoginId { get; set; }

		public DateTime? CreatedOnUtc { get; set; }
		public DateTime? ModifiedOnUtc { get; set; }

		public string FullName { get; set; }
		public string Email { get; set; }
		public bool Active { get; set; }

		public int TotalSecurityGroups { get; set; }

		[JsonIgnore]
		public CrudAction Action { get; set; }
	}

	public class AuthorizedUserBaseValidator<T> : AbstractValidator<T> where T : AuthorizedUserBaseModel
	{
		public AuthorizedUserBaseValidator()
		{
			RuleFor(a => a.Email)
				.NotNull().WithMessage("'Email' cannot be empty. Please provide a value for 'Email'.")
				.EmailAddress().WithMessage("'Email' is not in a valid email address format.");
		}
	}
}