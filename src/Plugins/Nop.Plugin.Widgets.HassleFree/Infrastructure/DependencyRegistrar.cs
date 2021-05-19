using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.HassleFree.Services.Agents;
using Nop.Plugin.Widgets.HassleFree.Services.Infrastructure;
using Nop.Plugin.Widgets.HassleFree.Services.Property24;

namespace Nop.Plugin.Widgets.HassleFree.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IGetProperty24Service, GetProperty24Service>();
            services.AddScoped<IListAgentService, ListAgentService>();
            services.AddScoped<ICloudStorageService, S3StorageService>();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
