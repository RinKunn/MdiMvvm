using System.Threading.Tasks;

namespace MdiExample.Services.WindowsServices.Store
{
    public interface IWindowStoreService
    {
        Task<bool> KeepAsync(string saveFileName = null);
    }
}
