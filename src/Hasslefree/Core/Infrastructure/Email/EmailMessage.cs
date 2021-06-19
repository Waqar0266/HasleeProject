using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Infrastructure.Email
{
    public class EmailMessage
    {
		#region Constructor
		public EmailMessage()
		{
			EmailRecipients = new List<string>();
			CcEmailRecipients = new List<string>();
			BccEmailRecipients = new List<string>();
			Attachments = new List<EmailAttachment>();
			Body = new EmailBody();
			Headers = new Dictionary<string, string>();
		}
		#endregion

        #region Value Properties
        public Dictionary<String, String> Headers { get; set; }
        public String FromName { get; set; }
        public String FromAddress { get; set; }
        public String ReplyAddress { get; set; }
        public List<String> EmailRecipients { get; set; }
        public List<String> CcEmailRecipients { get; set; }
        public List<String> BccEmailRecipients { get; set; }
        public EmailBody Body { get; set; }
        public String FromUrl { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
        public String Subject { get; set; }
        #endregion


    }
}
