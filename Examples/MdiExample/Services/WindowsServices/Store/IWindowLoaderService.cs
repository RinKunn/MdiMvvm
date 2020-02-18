using System.Threading.Tasks;

namespace MdiExample.Services.WindowsServices.Store
{
    public interface IWindowLoaderService
    {
        Task<bool> Load(string loadFileName = null);
    }
}
