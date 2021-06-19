using FluentValidation.Internal;

namespace Hasslefree.Web.Models.Security.AuthorizedUsers.ListFilters
{
	public static class AuthorizedUserSortByExtension
	{
		public static string GetDisplayName(this AuthorizedUserSortBy sortBy)
		{
			switch (sortBy)
			{
				case AuthorizedUserSortBy.Name:
					return AuthorizedUserSortBy.Name.ToString().SplitPascalCase();
				case AuthorizedUserSortBy.Email:
					return AuthorizedUserSortBy.Email.ToString().SplitPascalCase();
				case AuthorizedUserSortBy.SecurityGroupCountAsc:
					return AuthorizedUserSortBy.SecurityGroupCountAsc.ToString().SplitPascalCase();
				case AuthorizedUserSortBy.SecurityGroupCountDesc:
					return AuthorizedUserSortBy.SecurityGroupCountDesc.ToString().SplitPascalCase();
				default:
					return sortBy.ToString();
			}
		}
	}
}
