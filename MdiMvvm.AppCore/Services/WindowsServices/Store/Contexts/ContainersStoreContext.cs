﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Services.WindowsServices.Store
{
    public class ContainersStoreContext : IStoreContext
    {
        public Guid Guid { get; set; }
        public Type ViewModelType { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
        public ViewModelContext ViewModelContext { get; set; }
        public List<WindowsStoreContext> WindowsContextCollection { get; set; }

        public ContainersStoreContext()
        {
            ViewModelContext = new ViewModelContext();
            WindowsContextCollection = new List<WindowsStoreContext>();
        }
    }
}
