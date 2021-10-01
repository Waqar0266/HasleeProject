namespace Hasslefree.Core.Domain.Rentals
{
	public enum ExistingRentalStatus
	{
		PendingLandlordRegistration,
		PendingLandlordWitnessSignature,
		PendingAgentSignature,
		PendingAgentWitnessSignature,
		Completed
	}
}
