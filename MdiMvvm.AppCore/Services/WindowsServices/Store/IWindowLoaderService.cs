using System.Threading.Tasks;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IWindowLoaderService
    {
        Task<bool> LoadAsync(string loadFileName = null);
    }
}
