using NUnit.Framework;
using TerseControllerTesting.Controllers;

namespace TerseControllerTesting.Tests._05_CustomSyntax2
{
    [TestFixture]
    public class SomeOtherTests
    {
        [Test]
        public void Test_redirect_to_other_controller()
        {
            Ensure.That(new TestController())
                .Calling(c => c.Index())
                .RedirectsTo<PersonController>(c => c.Index);
        }
    }
}
