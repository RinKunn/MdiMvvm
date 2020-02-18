using System.Threading.Tasks;
using MdiExample.ViewModel.Base;
using MdiExample.Services.WindowsServices.Navigation;

namespace MdiExampleCoreTest.Services.Mocks
{
    public class MdiWindowMock : MdiWindowViewModelBase, INavigateAware
    {
        public string InternalText { get; set; }

        public MdiWindowMock() : base() { }

        public void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }

        protected override Task OnWindowKeepeng(ViewModelContext context)
        {
            context.AddValue("InternalText", InternalText);
            return Task.CompletedTask;
        }

        protected override Task OnWindowLoading(ViewModelContext context)
        {
            InternalText = context.GetValue<string>("InternalText");
            return Task.CompletedTask;
        }
    }
}
