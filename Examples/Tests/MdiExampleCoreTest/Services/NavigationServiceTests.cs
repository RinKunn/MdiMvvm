using System;
using System.Linq;
using MdiMvvm.AppCore.Tests.Services.Mocks;
using MdiMvvm.AppCore.Services.WindowsServices.Factory;
using MdiMvvm.AppCore.Services.WindowsServices.Navigation;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;
using MdiMvvm.AppCore.ViewModelsBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MdiMvvm.AppCore.Tests.Services
{
    [TestClass]
    public class NavigationServiceTests
    {
        private readonly WindowsSamples samples;
        private IWindowsManagerService managerService;
        private IWindowsFactory factoryService;
        private INavigationService navigationService;
        
        public NavigationServiceTests()
        {
            managerService = new WindowsManagerService();
            samples = new WindowsSamples(managerService);

            factoryService = new WindowsFactoryMock();
            navigationService = new NavigationService(managerService, factoryService);
        }

        [TestMethod]
        public void NavigateToAsyncExists_AtActiveContainer_ActiveContainerNotChanged()
        {
            Guid winGuid = samples.Guids[0].Value[0];
            ViewModelContext viewModelContext = new ViewModelContext();
            viewModelContext.AddValue<string>("Title", "New title");
            NavigateParameters navigateParameters = new NavigateParameters(viewModelContext, windowGuid: winGuid);

            navigationService.NavigateToAsync<MdiWindowMock>(navigateParameters);

            var aimWindow = managerService.Containers[0].WindowsCollection[0];
            Assert.AreEqual("Title1", managerService.ActiveContainer.Title);
            Assert.AreEqual(3, managerService.ActiveContainer.WindowsCollection.Count);
            Assert.AreEqual(winGuid, aimWindow.Guid);
            Assert.AreEqual("New title", aimWindow.Title);
            Assert.IsTrue(aimWindow.IsSelected);
        }

        [TestMethod]
        public void NavigateToAsyncExists_AtNotActiveContainer_ActiveContainerChanged()
        {
            Guid winGuid = samples.Guids[1].Value[0];
            ViewModelContext viewModelContext = new ViewModelContext();
            viewModelContext.AddValue<string>("Title", "New title");
            NavigateParameters navigateParameters = new NavigateParameters(viewModelContext, windowGuid: winGuid);

            navigationService.NavigateToAsync<MdiWindowMock>(navigateParameters);

            var aimWindow = managerService.Containers[1].WindowsCollection[0];
            Assert.AreEqual("Title2", managerService.ActiveContainer.Title);
            Assert.AreEqual(2, managerService.ActiveContainer.WindowsCollection.Count);
            Assert.AreEqual(winGuid, aimWindow.Guid);
            Assert.AreEqual("New title", aimWindow.Title);
            Assert.IsTrue(aimWindow.IsSelected);
        }

        [TestMethod]
        public void NavigateToAsyncNotExists_AtActiveContainer_ActiveContainerNotChangedAndCreatedNewWindow()
        {
            ViewModelContext viewModelContext = new ViewModelContext();
            viewModelContext.AddValue<string>("Title", "New title");
            NavigateParameters navigateParameters = new NavigateParameters(viewModelContext);

            navigationService.NavigateToAsync<MdiWindowMock>(navigateParameters);

            var aimWindow = managerService.Containers[0].WindowsCollection.Last();
            Assert.AreEqual("Title1", managerService.ActiveContainer.Title);
            Assert.AreEqual(4, managerService.ActiveContainer.WindowsCollection.Count);
            Assert.AreEqual("New title", aimWindow.Title);
            Assert.IsTrue(aimWindow.IsSelected);
        }

        [TestMethod]
        public void NavigateToAsyncNotExists_AtNotActiveContainer_ActiveContainerChangedAndCreatedNewWindow()
        {
            Guid contGuid = samples.Guids[1].Key;
            ViewModelContext viewModelContext = new ViewModelContext();
            viewModelContext.AddValue<string>("Title", "New title");
            NavigateParameters navigateParameters = new NavigateParameters(viewModelContext, containerGuid: contGuid);

            navigationService.NavigateToAsync<MdiWindowMock>(navigateParameters);

            var aimWindow = managerService.Containers[1].WindowsCollection.Last();
            Assert.AreEqual("Title2", managerService.ActiveContainer.Title);
            Assert.AreEqual(3, managerService.ActiveContainer.WindowsCollection.Count);
            Assert.AreEqual("New title", aimWindow.Title);
            Assert.IsTrue(aimWindow.IsSelected);
        }
    }
}
