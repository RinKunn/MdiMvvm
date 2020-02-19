using System;
using System.Collections.Generic;
using MdiExample.Services.WindowsServices.WindowsManager;
using MdiExampleCoreTest.Services.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MdiExampleCoreTest.Services
{
    [TestClass]
    public class ManagerServiceTests
    {
        private IWindowsManagerService manager;
        private List<Guid> guids;
        private List<Guid> winguids;

        public ManagerServiceTests()
        {
            guids = new List<Guid>();
            winguids = new List<Guid>();
            manager = new WindowsManagerService();

            guids.Add(manager.AppendContainer(new MdiContainerMock() { IsSelected = true, Title = "Title1" }).Guid);
            winguids.Add(manager.AppendWindowToContainer(new MdiWindowMock() { Title = "Window 1" }, guids[0]).Guid);
            winguids.Add(manager.AppendWindowToContainer(new MdiWindowMock() { Title = "Window 2" }, guids[0]).Guid);
            guids.Add(manager.AppendContainer(new MdiContainerMock() { Title = "Title2" }).Guid);
            guids.Add(manager.AppendContainer(new MdiContainerMock() { Title = "Title3" }).Guid);
            
        }

        [TestMethod]
        public void AppendNotSelectedContainer_ActiveContainerNotChanged()
        {
            MdiContainerMock container = new MdiContainerMock() { Title = "Title4", IsSelected = false };

            manager.AppendContainer(container);

            Assert.AreEqual(guids[0], manager.ActiveContainer.Guid);
            Assert.AreEqual("Title1", manager.ActiveContainer.Title);
        }

        [TestMethod]
        public void AppendSelectedContainer_ActiveContainerChanged()
        {
            MdiContainerMock container = new MdiContainerMock() { Title = "Title4", IsSelected = true };

            var guid = manager.AppendContainer(container).Guid;

            Assert.AreEqual(guid, manager.ActiveContainer.Guid);
            Assert.AreEqual("Title4", manager.ActiveContainer.Title);
        }

        [TestMethod]
        public void AppendWindowToActiveContainer_ActiveContainerNotChanged()
        {
            MdiWindowMock window = new MdiWindowMock();

            manager.AppendWindow(window);

            Assert.AreEqual("Title1", manager.ActiveContainer.Title);
            Assert.AreEqual(3, manager.ActiveContainer.WindowsCollection.Count);
        }

        [TestMethod]
        public void AppendWindowToOtherContainer_ActiveContainerChanged()
        {
            MdiWindowMock window = new MdiWindowMock();

            manager.AppendWindowToContainer(window, guids[1]);

            Assert.AreEqual("Title2", manager.ActiveContainer.Title);
            Assert.AreEqual(1, manager.ActiveContainer.WindowsCollection.Count);
        }

        [TestMethod]
        public void FindWindowForExistsWindow_ReturnExistsWindow()
        {
            Guid guid = winguids[0];

            var res = manager.FindWindow<MdiWindowMock>(guid);

            Assert.IsNotNull(res);
            Assert.AreEqual(guid, res.Guid);
            Assert.AreEqual("Window 1", res.Title);
        }

        [TestMethod]
        public void FindWindowForNotExistsWindow_ReturnNull()
        {
            Guid guid = Guid.NewGuid();

            var res = manager.FindWindow<MdiWindowMock>(guid);

            Assert.IsNull(res);
        }
    }
}
