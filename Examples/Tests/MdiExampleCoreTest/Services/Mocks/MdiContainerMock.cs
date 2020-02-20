using System.Threading.Tasks;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Tests.Services.Mocks
{
    public class MdiContainerMock : MdiContainerViewModelBase
    {
        public MdiContainerMock() : base() { }

        public override Task OnContainerKeeping(ViewModelContext context)
        {
            return Task.CompletedTask;
        }

        public override Task OnContainerLoading(ViewModelContext context)
        {
            return Task.CompletedTask;
        }
    }
}
