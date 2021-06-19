using System.Collections.Generic;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public interface IDeleteAuthorizedUserService
	{
		bool HasWarnings { get; }
		List<AuthorizedUserWarning> Warnings { get; }

		IDeleteAuthorizedUserService WithId(int loginId);
		IDeleteAuthorizedUserService WithIds(List<int> loginIds);
		
		/// <summary>
		/// Method used to delete a list of users from ALL their security groups
		/// </summary>
		/// <param name="saveChanges">Variable to save changes to datbase immediately after deleting</param>
		/// <returns></returns>
		bool Delete(bool saveChanges = true);
	}
}
