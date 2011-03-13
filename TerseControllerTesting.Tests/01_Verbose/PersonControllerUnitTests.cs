using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;

namespace TerseControllerTesting.Tests._01_Verbose
{
    [TestFixture]
    class PersonControllerUnitTests
    {
        private IPersonRepository _repository;
        private PersonController _controller;

        [SetUp]
        public void Setup()
        {
            _repository = Substitute.For<IPersonRepository>();
            _controller = new PersonController(_repository);
        }

        [Test]
        public void Should_render_index_view_with_get_to_index()
        {
            // Arrange

            // Act
            var viewResult = _controller.Index() as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo(""));
        }
    }
}
