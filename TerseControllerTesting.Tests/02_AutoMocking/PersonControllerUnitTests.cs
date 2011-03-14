using System.Collections.Generic;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Tests._02_AutoMocking
{
    [TestFixture]
    class PersonControllerUnitTests
    {

        #region Setup

        private AutoMock _autoMock;

        private readonly Person _person = new Person {FirstName = "FirstName", LastName = "LastName", EmailAddress = "email@email.com"};
        private const int PersonId = 1;

        [SetUp]
        public void Setup()
        {
            _autoMock = new AutoMock();
            _person.Id = 0;
        }

        #endregion

        #region Index

        [Test]
        public void Should_render_index_view_with_list_of_people_after_get_to_index()
        {
            // Arrange

            // Act
            var viewResult = _autoMock.Resolve<PersonController>().Index() as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo(""), "View was Index");

            var model = viewResult.Model as IEnumerable<Person>;
            Assert.That(model, Is.Not.Null, "Empty model was passed in");
        }

        #endregion

        #region Create

        [Test]
        public void Should_render_edit_view_after_get_to_create()
        {
            // Arrange

            // Act
            var viewResult = _autoMock.Resolve<PersonController>().Create() as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo("Edit"), "View was Edit");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_create_with_invalid_model()
        {
            // Arrange
            var controller = _autoMock.Resolve<PersonController>();
            controller.ModelState.AddModelError("Key", "Error");

            // Act
            var viewResult = controller.Create(_person) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo("Edit"), "View was Edit");

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_create_with_duplicate_email()
        {
            // Arrange
            var controller = _autoMock.Resolve<PersonController>();
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress).Returns(true);

            // Act
            var viewResult = controller.Create(_person) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo("Edit"), "View was Edit");

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(_person), "Model was passed through to view");

            Assert.That(controller.ModelState["EmailAddress"].Errors[0].ErrorMessage, Is.EqualTo("The Email address must be unique; that email address already exists in the system."), "There is a model state error against the email address field");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_create_with_valid_person()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>()).Returns(false);

            // Act
            var redirectResult = _autoMock.Resolve<PersonController>().Create(_person) as RedirectToRouteResult;

            // Assert
            Assert.That(redirectResult, Is.Not.Null, "Controller returned redirect");
            Assert.That(redirectResult.RouteValues["Action"], Is.EqualTo("Index"), "Redirected to index");
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
        }

        #endregion

        #region Edit

        [Test]
        public void Should_return_404_after_get_to_edit_with_invalid_id()
        {
            // Arrange

            // Act
            var result = _autoMock.Resolve<PersonController>().Edit(0) as HttpStatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Controller returned Http Status");
            Assert.That(result.StatusCode, Is.EqualTo(404), "Controller returned 404");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_get_to_edit_with_valid_id()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().GetById(PersonId).Returns(_person);

            // Act
            var viewResult = _autoMock.Resolve<PersonController>().Edit(PersonId) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo(""), "View was Edit");

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_edit_with_invalid_model()
        {
            // Arrange
            var controller = _autoMock.Resolve<PersonController>();
            controller.ModelState.AddModelError("Key", "Error");

            // Act
            var viewResult = controller.Edit(PersonId, _person) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo(""), "View was Edit");

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_edit_with_duplicate_email()
        {
            // Arrange
            var controller = _autoMock.Resolve<PersonController>();
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress, PersonId).Returns(true);

            // Act
            var viewResult = controller.Edit(PersonId, _person) as ViewResult;

            // Assert
            Assert.That(viewResult, Is.Not.Null, "Controller returned view");
            Assert.That(viewResult.ViewName, Is.EqualTo(""), "View was Edit");

            var model = viewResult.Model as Person;
            Assert.That(model, Is.SameAs(_person), "Model was passed through to view");

            Assert.That(controller.ModelState["EmailAddress"].Errors[0].ErrorMessage, Is.EqualTo("The Email address must be unique; that email address already exists in the system."), "There is a model state error against the email address field");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_edit_with_valid_person()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>(), Arg.Any<int>()).Returns(false);

            // Act
            var redirectResult = _autoMock.Resolve<PersonController>().Edit(PersonId, _person) as RedirectToRouteResult;

            // Assert
            Assert.That(redirectResult, Is.Not.Null, "Controller returned redirect");
            Assert.That(redirectResult.RouteValues["Action"], Is.EqualTo("Index"), "Redirected to index");
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
            Assert.That(_person.Id, Is.EqualTo(PersonId));
        }

        #endregion
    }
}
