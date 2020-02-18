using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MdiExample.ViewModel.Base;

namespace MdiExampleCoreTest.Services.Mocks
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
