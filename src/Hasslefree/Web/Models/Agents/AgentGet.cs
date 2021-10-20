using System.Collections.Generic;

namespace Hasslefree.Web.Models.Agents
{
	public class AgentGet
	{
		public AgentGet()
		{
			this.Documents = new List<AgentDocumentModel>();
			this.Forms = new List<AgentDocumentModel>();
		}

		public int AgentId { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string AgentTypeEnum { get; set; }
		public string AgentStatusEnum { get; set; }
		public List<AgentDocumentModel> Documents { get; set; }
		public List<AgentDocumentModel> Forms { get; set; }
		public AgentDocumentModel EaabProofOfPayment { get; set; }
	}
}
