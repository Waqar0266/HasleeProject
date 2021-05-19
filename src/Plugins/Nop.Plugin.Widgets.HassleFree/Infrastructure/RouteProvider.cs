using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.HassleFree.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            #region Registration

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.RegistrationRouteName, "register",
                new { controller = "HassleFreeRegistration", action = "Register", area = string.Empty });

            #endregion

            #region Configurations

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.AccountMenuConfigurationRouteName, "Plugins/HassleFree/Configure",
                new { controller = "WidgetsHassleFree", action = "Configure", area = AreaNames.Admin });

            #endregion

            #region Agents

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Agents.ListAgentRouteName, "customer/agents",
                new { controller = "ListAgent", action = "Index", area = string.Empty });

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Agents.AddAgentRouteName, "customer/agents/create",
                new { controller = "AddAgent", action = "Index", area = string.Empty });

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Agents.AgentDetailRouteName, "customer/agents/{id}",
                new { controller = "AgentDetail", action = "Index", area = string.Empty });
            
            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Agents.CompleteAgentRegistrationRouteName, "agents/complete-sign-up/{uniqueId}",
                new { controller = "CompleteAgentRegistration", action = "Index", area = string.Empty });

            #endregion

            #region Listings

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Listings.ListingsRouteName, "customer/listings",
                new { controller = "HassleFreeListings", action = "Index", area = string.Empty });

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Listings.Property24RouteName, "customer/listings/property24/{propertyId}",
                new { controller = "HassleFreeListings", action = "Property24", area = string.Empty });

            #endregion

            #region Forms

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Forms.FormsTestRouteName, "customer/forms/test",
                new { controller = "HassleFreeFormsTest", action = "Index", area = string.Empty });

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Forms.FormsSignaturePadRouteName, "customer/forms/sign/{uniqueId?}",
                new { controller = "Signature", action = "SignaturePad", area = string.Empty });

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Forms.FormsBondDEAConsentRouteName, "customer/forms/bond/deaconsent",
                new { controller = "HassleFreeFormsBonds", action = "DeaConsent", area = string.Empty });

            #endregion

            #region Media

            endpointRouteBuilder.MapControllerRoute(HassleFreeDefaults.Documents.UploadRouteName, "documents/upload",
                new { controller = "DocumentUpload", action = "Index", area = string.Empty });

            #endregion
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1; //set a value that is greater than the default one in Nop.Web to override routes
    }
}
