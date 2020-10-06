using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Controllers;
using WebStore.Interfaces.TestApi;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPITestControllerTests
    {
        //class TestValueService : IValueService
        //{
        //    public IEnumerable<string> Get() { throw new NotImplementedException(); }

        //    public string Get(int id) { throw new NotImplementedException(); }

        //    public Uri Post(string value) { throw new NotImplementedException(); }

        //    public HttpStatusCode Update(int id, string value) { throw new NotImplementedException(); }

        //    public HttpStatusCode Delete(int id) { throw new NotImplementedException(); }
        //}

        [TestMethod]
        public void Index_Returns_View_with_Values()
        {
            var expected_values = new[] { "1", "2", "3" };

            var value_service_mock = new Mock<IValueService>();

            value_service_mock
               .Setup(service => service.Get())
               .Returns(expected_values);

            //var service = new TestValueService();
            var controller = new WebAPITestController(value_service_mock.Object);

            var result = controller.Index();

            var view_result = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<string>>(view_result.Model);

            Assert.Equal(expected_values.Length, model.Count());

            // Если объект просто притворяется интерфейсом, то это "Стаб" (Stab)

            value_service_mock.Verify(service => service.Get());
            value_service_mock.VerifyNoOtherCalls();

            // Если выполняется последующая проверка состояния, то это "Мок" (Moq)
        }
    }
}
