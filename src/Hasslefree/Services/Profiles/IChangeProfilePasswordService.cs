using System.Collections.Generic;

namespace Hasslefree.Services.Profiles
{
	public interface IChangeProfilePasswordService
	{
		bool HasWarnings { get; }
		List<ChangeProfilePasswordWarning> Warnings { get; }

		IChangeProfilePasswordService WithCurrentPassword(string currentPassword);
		IChangeProfilePasswordService WithNewPassword(string newPassword);

		bool Update();
	}
}
