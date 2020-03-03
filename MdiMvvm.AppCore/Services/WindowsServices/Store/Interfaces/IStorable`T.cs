using System.Threading.Tasks;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public interface IStorable<TStoreContext> where TStoreContext : IStoreContext
    {
        void LoadFromStoreContext(TStoreContext context);
        TStoreContext InitStoreContext();
    }
}
