﻿using System;
using MdiExample.ViewModel.Base;
using MdiExample.Services.WindowsServices.Navigation;
using System.Threading.Tasks;

namespace MdiExample
{
    public class Window2ViewModel : MdiWindowViewModelBase, INavigateAware
    {
        public Window2ViewModel() : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
        }

        public void NavigatedTo(ViewModelContext context)
        {
            Title = context.GetValue<string>("Title");
        }

        protected override Task OnWindowKeepeng(ViewModelContext context)
        {
            return Task.CompletedTask;
        }

        protected override Task OnWindowLoading(ViewModelContext context)
        {
            return Task.CompletedTask;
        }
    }
}
