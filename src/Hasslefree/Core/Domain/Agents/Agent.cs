using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Agents
{
	public class Agent : BaseEntity
	{
		public Agent()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.AgentGuid = Guid.NewGuid();
			this.AgentStatus = AgentStatus.PendingRegistration;
		}

		public int AgentId { get; set; }
		public Guid AgentGuid { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string AgentTypeEnum { get; set; }
		public AgentType AgentType
		{
			get => (AgentType)Enum.Parse(typeof(AgentType), AgentTypeEnum);
			set => AgentTypeEnum = value.ToString();
		}
		public string AgentStatusEnum { get; set; }
		public AgentStatus AgentStatus
		{
			get => (AgentStatus)Enum.Parse(typeof(AgentStatus), AgentStatusEnum);
			set => AgentStatusEnum = value.ToString();
		}
		public string IdNumber { get; set; }
		public int? PersonId { get; set; }
		public Person Person { get; set; }
		public string Nationality { get; set; }
		public string Race { get; set; }
		public string PreviousEmployer { get; set; }
		public bool Ffc { get; set; }
		public string FfcNumber { get; set; }
		public DateTime? FfcIssueDate { get; set; }
		public string EaabReference { get; set; }
		public bool Dismissed { get; set; }
		public bool Convicted { get; set; }
		public bool Insolvent { get; set; }
		public bool Withdrawn { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
		public int? EaabProofOfPaymentId { get; set; }
		public Picture EaabProofOfPayment { get; set; }
		public string TempData { get; set; }
	}
}