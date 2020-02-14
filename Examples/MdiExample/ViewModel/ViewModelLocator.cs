using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using MdiMvvm.ViewModels;

namespace MdiExample.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    //public class ViewModelLocator
    //{
    //    private ViewModelLocator _instance;
    //    public ViewModelLocator Instance
    //    {
    //        get => _instance;
    //        set => _instance = value;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the ViewModelLocator class.
    //    /// </summary>
    //    public ViewModelLocator()
    //    {
    //        ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
    //    }

    //    public TWindowModel GetViewModel<TWindowModel>() where TWindowModel : MdiWindowViewModelBase
    //    {
    //        return SimpleIoc.Default.GetInstance<TWindowModel>();
    //    }

    //    public static void Cleanup()
    //    {
    //        // TODO Clear the ViewModels
    //    }
    //}
}