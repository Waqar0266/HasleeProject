using System;

namespace Hasslefree.Web.Models.Agents
{
	public class AgentDocumentModel
	{
		public int DownloadId { get; set; }
		public string Name { get; set; }
		public double Size { get; set; }
		public string Path { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
