using System.Threading.Tasks;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IStorable<TStoreContext> where TStoreContext : IStoreContext
    {
        Task<TStoreContext> OnLoading(TStoreContext context);
        Task<TStoreContext> OnKeeping(TStoreContext context);
    }
}
