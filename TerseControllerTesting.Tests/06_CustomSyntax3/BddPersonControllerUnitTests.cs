using System.Collections.Generic;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using TerseControllerTesting.Controllers;
using TerseControllerTesting.Data;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Tests._06_CustomSyntax3
{
    // ReSharper disable ConvertToLambdaExpression
    [TestFixture]
    public class BddPersonControllerUnitTests : NUnitControllerTest<PersonController>
    {

        private readonly Person _person = new Person { FirstName = "FirstName", LastName = "LastName", EmailAddress = "email@email.com" };
        private const int PersonId = 1;

        public override IEnumerable<ControllerTest<PersonController>>  TestSpecifications()
        {
            return _controller.Describe((callbacks, it) => {

                var autoMock = default(AutoMock);

                callbacks.Before = () =>
                {
                    autoMock = new AutoMock();
                    _controller = autoMock.Resolve<PersonController>();
                    _person.Id = 0;
                };

                it.Should("Render Index view with list of people after get to Index()", () => {
                    _controller.WithCallTo(c => c.Index())
                        .ShouldRenderDefaultView().WithModel<IEnumerable<Person>>();
                });

                it.Should("Render Edit view after get to Create()", () => {
                    _controller.WithCallTo(c => c.Create())
                        .ShouldRenderView("Edit");
                });

                it.Should("Render Edit view with person model after post to Create() with invalid model", () => {
                    _controller.WithModelErrors().WithCallTo(c => c.Create(_person))
                        .ShouldRenderView("Edit").WithModel(_person);
                });

                it.Should("Render Edit view with person model and error after post to Create() with duplicate email", () => {
                    autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress).Returns(true);
                    _controller.WithCallTo(c => c.Create(_person))
                        .ShouldRenderView("Edit")
                        .WithModel(_person)
                        .AndModelErrorFor(p => p.EmailAddress).ThatEquals("The Email address must be unique; that email address already exists in the system."); 
                });

                it.Should("Save and redirect to Index() after post to Create() with valid person", () => {
                    autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>()).Returns(false);
                    _controller.WithCallTo(c => c.Create(_person))
                        .ShouldRedirectTo(c => c.Index);
                    autoMock.Resolve<IPersonRepository>().Received().Save(_person);
                });

                it.Should("Give 404 after get to Edit() with invalid id", () => {
                    _controller.WithCallTo(c => c.Edit(0)).ShouldGiveHttpStatus(404);
                });

                it.Should("Render Edit view with person model after get to Edit() with valid id", () => {
                    autoMock.Resolve<IPersonRepository>().GetById(PersonId).Returns(_person);
                    _controller.WithCallTo(c => c.Edit(PersonId))
                        .ShouldRenderDefaultView().WithModel(_person);
                });

                it.Should("Render Edit view with person model after post to Edit() with invalid model", () => {
                    _controller.WithModelErrors().WithCallTo(c => c.Edit(PersonId, _person))
                        .ShouldRenderDefaultView().WithModel(_person);
                });

                it.Should("Render Edit view with person model and error after post to Edit() with duplicate email", () => {
                    autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(_person.EmailAddress, PersonId).Returns(true);
                    _controller.WithCallTo(c => c.Edit(PersonId, _person))
                        .ShouldRenderDefaultView().WithModel(_person)
                        .AndModelErrorFor(p => p.EmailAddress).ThatEquals("The Email address must be unique; that email address already exists in the system.");
                });

                it.Should("Save and redirect to Index() after post to edit with valid person", () => {
                    autoMock.Resolve<IPersonRepository>().EmailBelongsToSomeoneElse(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
                    _controller.WithCallTo(c => c.Edit(PersonId, _person));
                    autoMock.Resolve<IPersonRepository>().Received().Save(_person);
                    Assert.That(_person.Id, Is.EqualTo(PersonId));
                });
            });
        }
    }
}
