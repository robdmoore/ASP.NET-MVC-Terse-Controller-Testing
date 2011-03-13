using System.Collections.Generic;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;

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
        public void Should_render_index_view_with_list_of_people_with_get_to_index()
        {
            // Arrange

            // Act
            var viewResult = _controller.Index() as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo(""));

            var model = viewResult.Model as IEnumerable<Person>;
            Assert.That(model, Is.Not.Null);
        }

        [Test]
        public void Should_render_edit_view_with_get_to_create()
        {
            // Arrange

            // Act
            var viewResult = _controller.Create() as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("Edit"));
        }

        [Test]
        public void Should_return_404_with_get_to_edit_with_invalid_id()
        {
            // Arrange

            // Act
            var result = _controller.Edit(0) as HttpStatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void Should_return_edit_view_with_person_model_with_get_to_edit_with_valid_id()
        {
            // Arrange
            var person = new Person {FirstName = "FirstName", LastName = "LastName", EmailAddress = "email@email.com"};
            _repository.GetById(1).Returns(person);

            // Act
            var viewResult = _controller.Edit(1) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo(""));

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(person));
        }
    }
}
