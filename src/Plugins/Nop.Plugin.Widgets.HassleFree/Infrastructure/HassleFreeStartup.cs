using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.HassleFree.Infrastructure
{
    public class HassleFreeStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(new AWSOptions()
            {
                Credentials = new BasicAWSCredentials("AKIASRI2DWBM5BKVHJHQ","ryT/iwXAayxoWMQ+U5MerPGwSc4aJkPDku90ZNLU"),
                Region = RegionEndpoint.EUWest1
            });
            services.AddAWSService<IAmazonS3>();
        }

        public void Configure(IApplicationBuilder application)
        {

        }

        public int Order => 2;
    }
}
