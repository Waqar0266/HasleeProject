namespace Hasslefree.Services.Tenants.Crud
{
    public interface ICreateTenantAgentDocumentationService
    {
        ICreateTenantAgentDocumentationService Add(int rentalTId, int agentId, int downloadId);
        bool Process();
    }
}
