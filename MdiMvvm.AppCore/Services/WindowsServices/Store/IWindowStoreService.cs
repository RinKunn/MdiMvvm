using System.Threading.Tasks;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IWindowStoreService
    {
        Task<bool> KeepAsync(string saveFileName = null);
    }
}
