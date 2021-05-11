using System.Threading.Tasks;
using Nop.Plugin.Widgets.HassleFree.Models.Property24;

namespace Nop.Plugin.Widgets.HassleFree.Services.Property24
{
    public interface IGetProperty24Service
    {
        IGetProperty24Service WithPropertyId(string propertyId);
        Task<Property24Model> Get();
    }
}
