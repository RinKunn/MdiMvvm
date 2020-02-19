using MdiMvvm.Interfaces;

namespace MdiExample.Services.WindowsServices.WindowsManager
{
    public class ActiveContainerChangedArgs
    {
        public IMdiContainerViewModel OldContainer { get; private set; }
        public IMdiContainerViewModel NewContainer { get; private set; }

        public ActiveContainerChangedArgs(IMdiContainerViewModel oldcontainer, IMdiContainerViewModel newcontainer)
        {
            OldContainer = oldcontainer;
            NewContainer = newcontainer;
        }
    }
    public delegate void ActiveContainerChangedHandler(ActiveContainerChangedArgs args);

    public class ContainersCollectionChangedArgs
    {
        
        public ContainersCollectionChangedArgs()
        {
            
        }
    }

    public delegate void ContainersCollectionChangedHandler(ContainersCollectionChangedArgs args);
}
