namespace Hasslefree.Services.Tenants.Crud
{
    public interface ICreateTenantDocumentationService
    {
        ICreateTenantDocumentationService Add(int tenantId, int downloadId);
        bool Process();
    }
}
