using Hasslefree.Core.Infrastructure.Email;
using System;

namespace Hasslefree.Services.Infrastructure.Email
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public interface IEmailService
    {
        #region Methods
        void SendEmail(EmailMessage emailMessage);
        #endregion
    }
}
