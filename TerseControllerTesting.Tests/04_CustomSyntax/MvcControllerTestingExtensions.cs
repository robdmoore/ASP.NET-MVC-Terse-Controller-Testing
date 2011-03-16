using System;
using System.Web.Mvc;
using MvcContrib.TestHelper;

namespace TerseControllerTesting.Tests._04_CustomSyntax
{
    public static class MvcControllerTestingExtensions
    {
        public static ActionResult AssertHttpStatusReturned(this ActionResult actionResult)
        {
            var castedActionResult = actionResult as HttpStatusCodeResult;

            if (castedActionResult == null)
                throw new ActionResultAssertionException(string.Format("Expected action result to be a HttpStatusCodeResult, but instead received a {0}.", actionResult.GetType().Name));

            return actionResult;
        }

        public static ActionResult AssertHttpStatusReturned(this ActionResult actionResult, int status)
        {
            actionResult.AssertHttpStatusReturned();

            var castedActionResult = actionResult as HttpStatusCodeResult;

            if (castedActionResult.StatusCode != status)
                throw new ActionResultAssertionException(string.Format("Expected HTTP status code to be {0}, but instead received a {1}.", status, castedActionResult.StatusCode));

            return actionResult;
        }

        public static ViewResult WithModel<T>(this ViewResult viewResult) where T : class
        {
            var castedModel = viewResult.Model as T;
            if (castedModel == null)
                throw new ViewResultModelAssertionException(string.Format("Expected view model to be of type {0}. It is actually of type {1}.", typeof(T).Name, viewResult.Model.GetType().Name));

            return viewResult;
        }

        public static ViewResult WithModel<T>(this ViewResult viewResult, T expectedModel = default(T)) where T : class
        {
            viewResult.WithModel<T>();

            var model = viewResult.Model as T;
            if (model != expectedModel)
                throw new ViewResultModelAssertionException("Expected view model to be passed in model, but in fact it was a different model.");

            return viewResult;
        }

        public static Controller HasModelError(this Controller controller, string memberName, string equals = null, string contains = null)
        {
            if (!controller.ModelState.ContainsKey(memberName))
                throw new ViewResultModelAssertionException(string.Format("Expected controller {0} to have a model error for member {1}, but none found.", controller, memberName));

            if (!string.IsNullOrEmpty(equals) && controller.ModelState[memberName].Errors[0].ErrorMessage != equals)
                throw new ViewResultModelAssertionException(string.Format("Expected controller {0} to have a model error for member {1} that equals '{2}', but instead found: '{3}'.", controller, memberName, equals, controller.ModelState[memberName].Errors[0].ErrorMessage));

            if (!string.IsNullOrEmpty(contains) && controller.ModelState[memberName].Errors[0].ErrorMessage.Contains(contains))
                throw new ViewResultModelAssertionException(string.Format("Expected controller {0} to have a model error for member {1} that contains '{2}', but instead found: '{3}'.", controller, memberName, contains, controller.ModelState[memberName].Errors[0].ErrorMessage));

            return controller;
        }

        public static Controller AndModelError(this Controller controller, string memberName, string equals = null, string contains = null)
        {
            return controller.HasModelError(memberName, equals, contains);
        }

        public static T WithModelErrors<T>(this T controller) where T : Controller
        {
            controller.ModelState.AddModelError("key", "value");
            return controller;
        }
    }

    public class ViewResultModelAssertionException : Exception
    {
        public ViewResultModelAssertionException(string message) : base(message) {}
    }
}
