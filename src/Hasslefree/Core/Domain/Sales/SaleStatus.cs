namespace Hasslefree.Core.Domain.Sales
{
    public enum SaleStatus
    {
        PendingNew,
        PendingSellerRegistration,
        PendingSellerDocumentation,
        PendingSellerSignature,
        PendingSellerWitnessSignature,
        PendingAgentRegistration,
        PendingAgentDocumentation,
        PendingAgentSignature,
        PendingAgentWitnessSignature,
        PendingProperty24,
        PendingMemberSignatures,
        Completed
    }
}
