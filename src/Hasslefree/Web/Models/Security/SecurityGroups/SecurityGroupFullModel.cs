using FluentValidation.Attributes;
using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.SecurityGroups.Get;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups
{
	[Validator(typeof(SecurityGroupFullValidator))]
	public class SecurityGroupFullModel : SecurityGroupBaseModel
	{
		public SecurityGroupFullModel()
		{
			Members = new List<AuthorizedUserBaseModel>();
			Permissions = new List<SecurityGroupPermission>();
		}

		public List<AuthorizedUserBaseModel> Members { get; set; }
		public List<SecurityGroupPermission> Permissions { get; set; }
	}

	public class SecurityGroupFullValidator : SecurityGroupBaseValidator<SecurityGroupFullModel>
	{

	}
}
