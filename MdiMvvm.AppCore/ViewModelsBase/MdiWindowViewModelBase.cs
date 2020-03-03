using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.Store;

namespace MdiMvvm.AppCore.ViewModelsBase
{
    public abstract class MdiWindowViewModelBase : MdiWindowNotStorableViewModelBase, IStorable<WindowsStoreContext>
    {
        public MdiWindowViewModelBase() : base()
        {
            
        }

        public void LoadFromStoreContext(WindowsStoreContext context)
        {
            Guid = context.Guid;
            Title = context.Title;

            PreviousHeight = context.PreviousHeight;
            PreviousLeft = context.PreviousLeft;
            PreviousTop = context.PreviousTop;
            PreviousWidth = context.PreviousWidth;
            PreviousState = context.PreviousState;

            CurrentHeight = context.CurrentHeight;
            CurrentLeft = context.CurrentLeft;
            CurrentTop = context.CurrentTop;
            CurrentWidth = context.CurrentWidth;
            WindowState = context.WindowState;

            IsModal = context.IsModal;
            IsSelected = context.IsSelected;

            OnLoadingState(context.ViewModelContext);
        }

        public WindowsStoreContext InitStoreContext()
        {
            WindowsStoreContext context = new WindowsStoreContext();
            context.Guid = Guid;
            context.Title = Title;
            context.ViewModelType = GetType();

            context.PreviousHeight = PreviousHeight;
            context.PreviousLeft = PreviousLeft;
            context.PreviousTop = PreviousTop;
            context.PreviousWidth = PreviousWidth;
            context.PreviousState = PreviousState;

            context.CurrentHeight = CurrentHeight;
            context.CurrentLeft = CurrentLeft;
            context.CurrentTop = CurrentTop;
            context.CurrentWidth = CurrentWidth;
            context.WindowState = WindowState;

            context.IsModal = IsModal;
            context.IsSelected = IsSelected;
            context.ContainerGuid = Container.Guid;

            OnSavingState(context.ViewModelContext);

            return context;
        }

        protected abstract void OnLoadingState(ViewModelContext context);
        protected abstract void OnSavingState(ViewModelContext context);

    }
}
