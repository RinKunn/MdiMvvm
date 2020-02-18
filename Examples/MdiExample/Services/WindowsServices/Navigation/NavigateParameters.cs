using System;
using MdiExample.ViewModel.Base;

namespace MdiExample.Services.WindowsServices.Navigation
{
    public class NavigateParameters
    {
        public Guid GuidWindows { get; private set; }
        public Guid GuidContainer { get; private set; }
        public ViewModelContext Context { get; private set; }

        public NavigateParameters(ViewModelContext context)
        {
            GuidWindows = Guid.Empty;
            GuidContainer = Guid.Empty;
            Context = context;
        }

        //public NavigateParameters(Guid containerGuid, ViewModelContext context)
        //{
        //    GuidWindows = Guid.Empty;
        //    GuidContainer = containerGuid;
        //    Context = context;
        //}

        public NavigateParameters(ViewModelContext context, Guid windowGuid = default, Guid containerGuid = default)
        {
            GuidWindows = windowGuid;
            GuidContainer = containerGuid;
            Context = context;
        }
    }
}
