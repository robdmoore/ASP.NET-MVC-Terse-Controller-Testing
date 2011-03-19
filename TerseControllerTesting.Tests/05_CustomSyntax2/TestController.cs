using System.Web.Mvc;
using System.Web.Routing;

namespace TerseControllerTesting.Tests._05_CustomSyntax2
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Person", Action = "Index" }));
        }

    }
}
