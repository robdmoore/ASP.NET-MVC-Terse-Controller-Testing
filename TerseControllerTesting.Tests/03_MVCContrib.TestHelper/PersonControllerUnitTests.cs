using System.Collections.Generic;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;
using MvcContrib.TestHelper;

namespace TerseControllerTesting.Tests._03_MVCContrib.TestHelper
{
    [TestFixture]
    class PersonControllerUnitTests
    {

        #region Setup

        private AutoMock _autoMock;
        private PersonController _controller;
        private readonly Person _person = new Person {FirstName = "FirstName", LastName = "LastName", EmailAddress = "email@email.com"};
        private const int PersonId = 1;

        [SetUp]
        public void Setup()
        {
            _autoMock = new AutoMock();
            _controller = _autoMock.Resolve<PersonController>();
            _person.Id = 0;
        }

        #endregion

        #region Index

        [Test]
        public void Should_render_index_view_with_list_of_people_after_get_to_index()
        {
            var viewResult = _controller.Index().AssertViewRendered().ForView("");
            Assert.That(viewResult.Model as IEnumerable<Person>, Is.Not.Null, "Empty model was passed in");
        }

        #endregion

        #region Create

        [Test]
        public void Should_render_edit_view_after_get_to_create()
        {
            _controller.Create().AssertViewRendered().ForView("Edit");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_create_with_invalid_model()
        {
            // Arrange
            _controller.ModelState.AddModelError("Key", "Error");

            // Act & Assert
            var viewResult = _controller.Create(_person).AssertViewRendered().ForView("Edit");
            Assert.That(viewResult.Model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_create_with_duplicate_email()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress).Returns(true);

            // Act & Assert
            var viewResult = _controller.Create(_person).AssertViewRendered().ForView("Edit");
            Assert.That(viewResult.Model, Is.SameAs(_person), "Model was passed through to view");
            Assert.That(_controller.ModelState["EmailAddress"].Errors[0].ErrorMessage, Is.EqualTo("The Email address must be unique; that email address already exists in the system."), "There is a model state error against the email address field");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_create_with_valid_person()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>()).Returns(false);

            // Act & Assert
            _controller.Create(_person).AssertActionRedirect().ToAction("Index");
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
        }

        #endregion

        #region Edit

        [Test]
        public void Should_return_404_after_get_to_edit_with_invalid_id()
        {
            // Arrange

            // Act
            var result = _controller.Edit(0) as HttpStatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Controller returned Http Status");
            Assert.That(result.StatusCode, Is.EqualTo(404), "Controller returned 404");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_get_to_edit_with_valid_id()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().GetById(PersonId).Returns(_person);

            // Act & Assert
            var viewResult = _controller.Edit(PersonId).AssertViewRendered().ForView("");
            Assert.That(viewResult.Model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_edit_with_invalid_model()
        {
            // Arrange
            _controller.ModelState.AddModelError("Key", "Error");

            // Act & Assert
            var viewResult = _controller.Edit(PersonId, _person).AssertViewRendered().ForView("");
            Assert.That(viewResult.Model, Is.SameAs(_person), "Model was passed through to view");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_edit_with_duplicate_email()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress, PersonId).Returns(true);

            // Act & Assert
            var viewResult = _controller.Edit(PersonId, _person).AssertViewRendered().ForView("");
            Assert.That(viewResult.Model, Is.SameAs(_person), "Model was passed through to view");
            Assert.That(_controller.ModelState["EmailAddress"].Errors[0].ErrorMessage, Is.EqualTo("The Email address must be unique; that email address already exists in the system."), "There is a model state error against the email address field");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_edit_with_valid_person()
        {
            // Arrange
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>(), Arg.Any<int>()).Returns(false);

            // Act & Assert
            _controller.Edit(PersonId, _person).AssertActionRedirect().ToAction("Index");
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
            Assert.That(_person.Id, Is.EqualTo(PersonId));
        }

        #endregion
    }
}
