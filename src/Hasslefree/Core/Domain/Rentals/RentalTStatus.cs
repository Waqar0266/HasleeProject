namespace Hasslefree.Core.Domain.Rentals
{
	public enum RentalTStatus
	{
		PendingNew,
        PendingTenantDocumentation,
        PendingTenantSignature,
        PendingAgentDocumentation,
        PendingLandlordApproval,
        PendingAgentApproval
    }
}
