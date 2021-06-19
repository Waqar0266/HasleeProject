using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Core.Domain.Agents
{
	public class AgentAddress
	{
		public int AgentAddressId { get; set; }
		public int AgentId { get; set; }
		public Agent Agent { get; set; }
		public int AddressId { get; set; }
		public Address Address { get; set; }
	}
}
