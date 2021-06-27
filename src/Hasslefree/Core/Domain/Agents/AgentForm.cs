using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Agents
{
	public class AgentForm : BaseEntity
	{
		public AgentForm()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int AgentFormId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int AgentId { get; set; }
		public Agent Agent { get; set; }
		public int DownloadId { get; set; }
		public Download Download { get; set; }
		public string FormNameEnum { get; set; }
		public FormName FormName
		{
			get => (FormName)Enum.Parse(typeof(FormName), FormNameEnum);
			set => FormNameEnum = value.ToString();
		}
	}
}
