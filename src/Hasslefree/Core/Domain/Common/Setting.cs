using System;

namespace Hasslefree.Core.Domain.Common
{
	public class Setting : BaseEntity
	{
		public Setting()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
		}

		public int SettingId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
