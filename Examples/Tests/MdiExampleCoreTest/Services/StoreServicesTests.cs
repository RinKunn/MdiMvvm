using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using MdiExample.Services.WindowsServices.Factory;
using MdiExample.Services.WindowsServices.Store;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiExampleCoreTest.Services.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MdiExampleCoreTest.Services
{
    [TestClass]
    public class StoreServicesTests
    {
        private readonly IWindowsManagerService managerService;
        private readonly IWindowStoreService storeService;
        private readonly IWindowsFactory factoryService;

        static StoreServicesTests()
        {
            DispatcherHelper.Initialize();
        }

        public  StoreServicesTests()
        {
            managerService = new WindowsManagerService();
            var samples = new WindowsSamples(managerService);
            factoryService = new WindowsFactoryMock();
            storeService = new JsonWindowStoreService(managerService);
        }

        [TestMethod]
        public async Task StoreWindowsManagerStates_NotEmptyFileCreated()
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "testsettings.txt");
            await storeService.Keep(filename);

            Assert.IsTrue(File.Exists(filename));
            Assert.AreNotEqual(0, (new FileInfo(filename)).Length);
            //File.Delete(filename);
        }

        [TestMethod]
        public async Task LoadWindowsManagerStates_ValidLoaded()
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "testsettings2.txt");
            await storeService.Keep(filename);
            var newManagerService = new WindowsManagerService();
            var loaderService = new JsonWindowLoaderService(newManagerService, factoryService);

            await loaderService.LoadAsync(filename);

            Assert.IsNotNull(newManagerService.Containers);
            Assert.AreEqual("Title1", newManagerService.ActiveContainer.Title);
            Assert.AreEqual(3, newManagerService.Containers.Count);
            Assert.AreEqual(3, newManagerService.Containers[0].WindowsCollection.Count);
            Assert.AreEqual(2, newManagerService.Containers[1].WindowsCollection.Count);
            Assert.AreEqual(1, newManagerService.Containers[2].WindowsCollection.Count);
            var wind = (MdiWindowMock)newManagerService.Containers[2].WindowsCollection[0];
            Assert.AreEqual("Internal text", wind.InternalText);
        }
    }
}
