using System;
using System.Threading.Tasks;
using MdiMvvm.AppCore.Services.WindowsServices.Store;
using MdiMvvm.AppCore.ViewModelsBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MdiMvvm.AppCore.Tests.ViewModelsBase
{
    [TestClass]
    public class ViewModelContextTests
    {
        JsonSerializerSettings settings;
        string filename = @".\store.json";

        public ViewModelContextTests()
        {
            settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        [TestMethod]
        public async Task LoadString_ReturnString()
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue<string>("id", "textid");
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.GetValue<string>("id");

            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual("textid", result);
        }

        [TestMethod]
        public async Task LoadDateTime_ReturnDateTime()
        {
            DateTime now = DateTime.Now;
            ViewModelContext context = new ViewModelContext();
            context.AddValue<DateTime>("id", now);
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.GetValue<DateTime>("id");

            Assert.IsInstanceOfType(result, typeof(DateTime));
            Assert.AreEqual(now, result);
        }

        [TestMethod]
        public async Task LoadClass_ReturnClass()
        {
            TestClass obj = new TestClass(19);
            ViewModelContext context = new ViewModelContext();
            context.AddValue<TestClass>("id", obj);
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.GetValue<TestClass>("id");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TestClass));
            //Assert.AreEqual(obj, result);
            Assert.AreEqual(19, result.Id);
            Assert.AreEqual(DateTime.Today, result.DateTime.Date);
            Assert.AreEqual("Id is 19", result.Text);
        }

        [TestMethod]
        public async Task LoadChar_ReturnChar()
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue<char>("id", 's');
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.GetValue<char>("id");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(char));
            Assert.AreEqual('s', result);
        }

        [TestMethod]
        public async Task LoadInt_ReturnInt()
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue<int>("id", 158);
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.GetValue<int>("id");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.AreEqual(158, result);
        }

        [TestMethod]
        public async Task LoadIntForString_ReturnFalse()
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue<string>("id", "qj123");
            await context.SaveObjectToJsonFileAsync(filename, settings);

            var newcontext = await filename.GetObjectFromJsonFileAsync<ViewModelContext>(settings);
            var result = newcontext.TryGetValue<int>("id", out int resInt);

            Assert.AreEqual(0, resInt);
            Assert.AreEqual(false, result);
            
        }
    }

    public class TestClass
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

        public TestClass(int id)
        {
            Id = id;
            Text = $"Id is {id}";
            DateTime = DateTime.Now;
        }
    }
}
