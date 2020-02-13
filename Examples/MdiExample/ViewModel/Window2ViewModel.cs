using System;
using MdiMvvm.ViewModels;
using GalaSoft.MvvmLight;

namespace MdiExample
{
    public class Window2ViewModel : MdiWindowViewModelBase
    {
        public Window2ViewModel() : base()
        {
            Random r = new Random();
            Title = $"Window {r.Next(1, 1000)}";
        }
    }
}
