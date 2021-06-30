using Hasslefree.Core.Domain.Agents;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Agents
{
	public class AgentModel
	{
		public int AgentId { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string IdNumber { get; set; }
		public AgentType AgentType { get; set; }

		public List<AgentDocumentModel> Documents { get; set; }
		public List<AgentDocumentModel> Forms { get; set; }
	}
}
