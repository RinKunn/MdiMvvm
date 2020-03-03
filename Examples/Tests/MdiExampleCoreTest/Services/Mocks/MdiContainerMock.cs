using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Tests.Services.Mocks
{
    public class MdiContainerMock : MdiContainerViewModelBase
    {
        public MdiContainerMock() : base() { }


        protected override void OnLoadingContainerState(ViewModelContext context)
        {
            
        }

        protected override void OnSavingContainerState(ViewModelContext context)
        {
            
        }
    }
}
