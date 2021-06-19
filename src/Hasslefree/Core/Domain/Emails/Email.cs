using Hasslefree.Core.Helpers.Extensions;
using System;

namespace Hasslefree.Core.Domain.Emails
{
	public class Email : BaseEntity
	{
		#region Constructor

		public Email()
		{
			Send = false;
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			SendType = SendType.Default;
		}

		#endregion

		#region Value Properties

		public Int32 EmailId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public String Type { get; set; }
		public Boolean Send { get; set; }
		public String SendTypeEnum { get; set; }
		public String From { get; set; }
		public String Subject { get; set; }
		public String Url { get; set; }
		public String Recipient { get; set; }
		public String Template { get; set; }

		#endregion

		#region Enumeration Properties

		public SendType SendType
		{
			get => SendTypeEnum.ToEnum<SendType>();
			set => SendTypeEnum = value.ToString();
		}

		#endregion
	}
}
