using Hasslefree.Web.Models.Security.SecurityGroups;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.AuthorizedUsers
{
	public class AuthorizedUserFullModel : AuthorizedUserBaseModel
	{
		public AuthorizedUserFullModel()
		{
			SecurityGroups = new List<SecurityGroupBaseModel>();
		}

		public List<SecurityGroupBaseModel> SecurityGroups { get; set; }
	}
}
