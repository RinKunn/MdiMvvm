using System.Threading.Tasks;
using MdiExample.Services.WindowsServices.Store;

namespace MdiExample.Services.WindowsServices.Store
{
    public interface IStorable<TStoreContext> where TStoreContext : IStoreContext
    {
        Task<TStoreContext> OnLoading(TStoreContext context);
        Task<TStoreContext> OnKeeping(TStoreContext context);
    }
}
