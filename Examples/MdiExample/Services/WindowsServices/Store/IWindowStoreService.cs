using System.Threading.Tasks;

namespace MdiExample.Services.WindowsServices.Store
{
    public interface IWindowStoreService
    {
        Task<bool> Keep(string saveFileName = null);
    }
}
