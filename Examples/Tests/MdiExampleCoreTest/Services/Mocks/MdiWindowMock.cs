﻿using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Tests.Services.Mocks
{
    public class MdiWindowMock : MdiWindowViewModelBase
    {
        public string InternalText { get; set; }

        public MdiWindowMock() : base() { }

        public override void NavigatedTo(ViewModelContext context)
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
