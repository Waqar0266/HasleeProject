using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Agents
{
	public class AgentDocumentation : BaseEntity
	{
		public AgentDocumentation()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int AgentDocumentationId { get; set; }
		public int AgentId { get; set; }
		public Agent Agent { get; set; }
		public int DownloadId { get; set; }
		public Download Download { get; set; }
		public DateTime CreatedOn { get; set; }
		
	}
}
