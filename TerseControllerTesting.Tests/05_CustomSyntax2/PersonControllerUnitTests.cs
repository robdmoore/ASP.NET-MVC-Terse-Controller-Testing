using System.Collections.Generic;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Tests._05_CustomSyntax2
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
            Ensure.That(_controller).Calling(c => c.Index()).RendersDefaultView().WithModel<IEnumerable<Person>>();
        }

        #endregion

        #region Create

        [Test]
        public void Should_render_edit_view_after_get_to_create()
        {
            Ensure.That(_controller).Calling(c => c.Create()).RendersView("Edit");
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_create_with_invalid_model()
        {
            Ensure.That(_controller).WithModelErrors().Calling(c => c.Create(_person))
                .RendersView("Edit").WithModel(_person);
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_create_with_duplicate_email()
        {
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress).Returns(true);
            Ensure.That(_controller).Calling(c => c.Create(_person))
                .RendersView("Edit")
                .WithModel(_person)
                .AndModelErrorFor(p => p.EmailAddress).ThatEquals("The Email address must be unique; that email address already exists in the system.");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_create_with_valid_person()
        {
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>()).Returns(false);
            Ensure.That(_controller).Calling(c => c.Create(_person)).RedirectsTo(c => c.Index);
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
        }

        #endregion

        #region Edit

        [Test]
        public void Should_return_404_after_get_to_edit_with_invalid_id()
        {
            Ensure.That(_controller).Calling(c => c.Edit(0)).ReturnsHttpStatus(404);
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_get_to_edit_with_valid_id()
        {
            _autoMock.Resolve<IPersonRepository>().GetById(PersonId).Returns(_person);
            Ensure.That(_controller).Calling(c => c.Edit(PersonId)).RendersDefaultView().WithModel(_person);
        }

        [Test]
        public void Should_return_edit_view_with_person_model_after_post_to_edit_with_invalid_model()
        {
            Ensure.That(_controller).WithModelErrors().Calling(c => c.Edit(PersonId, _person))
                .RendersDefaultView().WithModel(_person);
        }

        [Test]
        public void Should_return_edit_view_with_person_model_and_error_after_post_to_edit_with_duplicate_email()
        {
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress, PersonId).Returns(true);
            Ensure.That(_controller).Calling(c => c.Edit(PersonId, _person))
                .RendersDefaultView().WithModel(_person)
                .AndModelErrorFor(p => p.EmailAddress).ThatEquals("The Email address must be unique; that email address already exists in the system.");
        }

        [Test]
        public void Should_save_and_redirect_to_index_after_post_to_edit_with_valid_person()
        {
            _autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
            Ensure.That(_controller).Calling(c => c.Edit(PersonId, _person)).RedirectsTo(c => c.Index);
            _autoMock.Resolve<IPersonRepository>().Received().Save(_person);
            Assert.That(_person.Id, Is.EqualTo(PersonId));
        }

        #endregion
    }
}
