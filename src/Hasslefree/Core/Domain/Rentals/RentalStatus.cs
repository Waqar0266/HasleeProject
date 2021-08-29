namespace Hasslefree.Core.Domain.Rentals
{
	public enum RentalStatus
	{
		PendingNew,
		PendingLandlordRegistration,
		PendingLandlordDocumentation,
		PendingLandlordSignature,
		PendingLandlordWitnessSignature,
		PendingAgentRegistration,
		PendingAgentDocumentation,
		PendingAgentSignature,
		PendingAgentWitnessSignature,
		PendingProperty24,
		PendingPartnerSignatures,
		Completed
	}
}
