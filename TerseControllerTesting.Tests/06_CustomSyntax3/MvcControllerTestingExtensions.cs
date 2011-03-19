using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NUnit.Framework;
using TerseControllerTesting.Controllers;

namespace TerseControllerTesting.Tests._06_CustomSyntax3
{
    #region ControllerResultTest

    public class ControllerResultTest<T> where T: Controller
    {
        #region Private properties and methods
        private readonly T _controller;
        private readonly string _actionName;
        private readonly ActionResult _actionResult;

        private void ValidateActionReturnType<TActionResult>() where TActionResult : ActionResult
        {
            var castedActionResult = _actionResult as TActionResult;

            if (castedActionResult == null)
                throw new ActionResultAssertionException(
                    string.Format("Expected action result to be a {0}, but instead received a {1}.",
                        typeof(TActionResult).Name, _actionResult.GetType().Name
                    )
                );
        }
        #endregion

        #region Ctor

        public ControllerResultTest(T controller, string actionName, ActionResult actionResult)
        {
            _controller = controller;
            _actionName = actionName;
            _actionResult = actionResult;
        }

        #endregion

        #region Redirects

        public void ShouldRedirectTo(string url)
        {
            ValidateActionReturnType<RedirectResult>();
            var redirectResult = (RedirectResult)_actionResult;

            if (redirectResult.Url != url)
                throw new ActionResultAssertionException(string.Format("Expected redirect to URL '{0}', but instead was given a redirect to URL '{1}'.", url, redirectResult.Url));
        }

        public void ShouldRedirectTo(Func<T, Func<ActionResult>> actionRedirectedTo)
        {
            ValidateActionReturnType<RedirectToRouteResult>();

            var controllerName = new Regex(@"Controller$").Replace(typeof (T).Name, "");
            var actionName = actionRedirectedTo(_controller).Method.Name;
            var redirectResult = (RedirectToRouteResult) _actionResult;

            if (redirectResult.RouteValues.ContainsKey("Controller") && redirectResult.RouteValues["Controller"].ToString() != controllerName)
                throw new ActionResultAssertionException(string.Format("Expected redirect to controller '{0}', but instead was given a redirect to controller '{1}'.", controllerName, redirectResult.RouteValues["Controller"]));

            if (redirectResult.RouteValues["Action"].ToString() != actionName)
                throw new ActionResultAssertionException(string.Format("Expected redirect to action '{0}', but instead was given a redirect to action '{1}'.", actionName, redirectResult.RouteValues["Action"]));
        }

        public void ShouldRedirectTo<TController>(Expression<Func<TController, Func<ActionResult>>> actionRedirectedTo) where TController: Controller
        {
            ValidateActionReturnType<RedirectToRouteResult>();

            var controllerName = new Regex(@"Controller$").Replace(typeof(TController).Name, "");
            var methodInfo = (MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)(actionRedirectedTo.Body)).Operand).Arguments[2]).Value;
            var actionName = methodInfo.Name;

            var redirectResult = (RedirectToRouteResult)_actionResult;

            if (redirectResult.RouteValues["Controller"].ToString() != controllerName)
                throw new ActionResultAssertionException(string.Format("Expected redirect to controller '{0}', but instead was given a redirect to controller '{1}'.", controllerName, redirectResult.RouteValues["Controller"]));

            if (redirectResult.RouteValues["Action"].ToString() != actionName)
                throw new ActionResultAssertionException(string.Format("Expected redirect to action '{0}', but instead was given a redirect to action '{1}'.", actionName, redirectResult.RouteValues["Action"]));
        }

        #endregion

        #region View Results

        public ViewResultTest ShouldRenderView(string viewName)
        {
            ValidateActionReturnType<ViewResult>();

            var viewResult = (ViewResult)_actionResult;

            if (viewResult.ViewName != viewName && (viewName != _actionName || viewResult.ViewName != ""))
            {
                throw new ActionResultAssertionException(string.Format("Expected result view to be '{0}', but instead was given '{1}'.", viewName, viewResult.ViewName == "" ? _actionName : viewResult.ViewName));
            }

            return new ViewResultTest(viewResult, _controller);
        }

        public ViewResultTest ShouldRenderDefaultView()
        {
            ValidateActionReturnType<ViewResult>();

            var viewResult = (ViewResult)_actionResult;

            if (viewResult.ViewName != "" && viewResult.ViewName != _actionName)
            {
                throw new ActionResultAssertionException(string.Format("Expected result view to be '{0}', but instead was given '{1}'.", _actionName, viewResult.ViewName));
            }

            return new ViewResultTest(viewResult, _controller);
        }

        #endregion

        #region Http Status

        public void ShouldGiveHttpStatus()
        {
            ValidateActionReturnType<HttpStatusCodeResult>();
        }

        public void ShouldGiveHttpStatus(int status)
        {
            ValidateActionReturnType<HttpStatusCodeResult>();

            var statusCodeResult = (HttpStatusCodeResult)_actionResult;

            if (statusCodeResult.StatusCode != status)
                throw new ActionResultAssertionException(string.Format("Expected HTTP status code to be '{0}', but instead received a '{1}'.", status, statusCodeResult.StatusCode));
        }

        #endregion
    }

    #endregion

    #region BDD-like Syntax
    public static class BddControllerExtensions
    {
        public static IEnumerable<ControllerTest<T>> Describe<T>(this T controller, ControllerTestSpecifications<T> testSpecifications) where T : Controller
        {
            var factory = new ControllerTestFactory<T>(typeof(T).Name);
            testSpecifications(factory.Callbacks, factory);
            return factory.Tests;
        }
    }

    public class ControllerTestFactory<T> where T : Controller
    {
        private readonly string _scenario;

        public List<ControllerTest<T>> Tests { get; set; }
        public ControllerTestCallbacks Callbacks { get; set; }

        public ControllerTestFactory(string scenario)
        {
            _scenario = scenario;
            Tests = new List<ControllerTest<T>>();
            Callbacks = new ControllerTestCallbacks();
        }

        public void Should(string description, Callback test)
        {
            Tests.Add(new ControllerTest<T>(_scenario, description, Callbacks.Before, Callbacks.After, test));
        }
    }

    public delegate void Callback();
    public delegate void ControllerTestSpecifications<T>(ControllerTestCallbacks callbacks, ControllerTestFactory<T> factory) where T : Controller;

    public class ControllerTestCallbacks
    {
        public Callback Before { get; set; }
        public Callback After { get; set; }
    }

    public class ControllerTest<T> where T : Controller
    {
        private readonly string _scenario;
        private readonly string _description;
        private readonly Callback _before;
        private readonly Callback _after;
        private readonly Callback _test;

        public ControllerTest(string scenario, string description, Callback before, Callback after, Callback test)
        {
            _scenario = scenario;
            _description = description;
            _before = before;
            _after = after;
            _test = test;
        }

        public void Run()
        {
            if (_before != null)
                _before();

            _test();

            if (_after != null)
                _after();
        }

        public override string ToString()
        {
            return string.Format(@"Controller {0} Should {1}", _scenario, _description);
        }
    }

    public class NUnitControllerTest<T> where T: Controller
    {
        // ReSharper disable InconsistentNaming
        protected T _controller;
        // ReSharper restore InconsistentNaming

        [TestCaseSource("TestSpecifications")]
        public void Test(ControllerTest<T> test)
        {
            test.Run();
        }

        public virtual IEnumerable<ControllerTest<PersonController>> TestSpecifications()
        {
            yield break;
        }
    }

    #endregion

    #region Controller Extensions
    public static class ControllerExtensions
    {

        public static T WithModelErrors<T>(this T controller) where T : Controller
        {
            controller.ModelState.AddModelError("Key", "Value");
            return controller;
        }

        public static ControllerResultTest<T> WithCallTo<T, TAction>(this T controller, Expression<Func<T, TAction>> actionCall)
            where T : Controller
            where TAction : ActionResult
        {
            var actionName = ((MethodCallExpression)actionCall.Body).Method.Name;

            var actionResult = actionCall.Compile().Invoke(controller);

            return new ControllerResultTest<T>(controller, actionName, actionResult);
        }
    }
    #endregion

    #region ViewResultTest

    public class ViewResultTest
    {
        private readonly ViewResult _viewResult;
        private readonly Controller _controller;

        public ViewResultTest(ViewResult viewResult, Controller controller)
        {
            _viewResult = viewResult;
            _controller = controller;
        }

        public ModelTest<TModel> WithModel<TModel>() where TModel : class
        {
            var castedModel = _viewResult.Model as TModel;
            if (castedModel == null)
                throw new ViewResultModelAssertionException(string.Format("Expected view model to be of type '{0}'. It is actually of type '{1}'.", typeof(TModel).Name, _viewResult.Model.GetType().Name));

            return new ModelTest<TModel>(_controller);
        }

        public ModelTest<TModel> WithModel<TModel>(TModel expectedModel = default(TModel)) where TModel : class
        {
            var test = WithModel<TModel>();

            var model = _viewResult.Model as TModel;
            if (model != expectedModel)
                throw new ViewResultModelAssertionException("Expected view model to be passed in model, but in fact it was a different model.");

            return test;
        }
    }

    #endregion

    #region ModelTest

    public class ModelTest<TModel>
    {
        private readonly Controller _controller;

        public ModelTest(Controller controller)
        {
            _controller = controller;
        }

        public ModelErrorTest<TModel> AndModelErrorFor<TAttribute>(Expression<Func<TModel, TAttribute>> memberWithError)
        {
            var member = ((MemberExpression)memberWithError.Body).Member.Name;
            if (!_controller.ModelState.ContainsKey(member) || _controller.ModelState[member].Errors.Count == 0)
                throw new ViewResultModelAssertionException(string.Format("Expected controller '{0}' to have a model error for member '{1}', but none found.", _controller, member));
            return new ModelErrorTest<TModel>(this, member, _controller.ModelState[member].Errors);
        }

        public ModelErrorTest<TModel> AndModelError(string errorKey)
        {
            if (!_controller.ModelState.ContainsKey(errorKey) || _controller.ModelState[errorKey].Errors.Count == 0)
                throw new ViewResultModelAssertionException(string.Format("Expected controller '{0}' to have a model error against key '{1}', but none found.", _controller, errorKey));
            return new ModelErrorTest<TModel>(this, errorKey, _controller.ModelState[errorKey].Errors);
        }
    }

    #endregion

    #region ModelErrorTest

    public class ModelErrorTest<TModel>
    {
        private readonly ModelTest<TModel> _modelTest;
        private readonly string _errorKey;
        private readonly string _error;

        public ModelErrorTest(ModelTest<TModel> modelTest, string errorKey, ModelErrorCollection errors)
        {
            _modelTest = modelTest;
            _errorKey = errorKey;
            _error = errors[0].ErrorMessage;
        }

        public ModelTest<TModel> ThatEquals(string errorMessage)
        {
            if (_error != errorMessage)
            {
                throw new ModelErrorAssertionException(string.Format("Expected error message for key '{0}' to be '{1}', but instead found '{2}'.", _errorKey, errorMessage, _error));
            }
            return _modelTest;
        }

        public ModelTest<TModel> BeginningWith(string beginMessage)
        {
            if (!_error.StartsWith(beginMessage))
            {
                throw new ModelErrorAssertionException(string.Format("Expected error message for key '{0}' to start with '{1}', but instead found '{2}'.", _errorKey, beginMessage, _error));
            }
            return _modelTest;
        }

        public ModelTest<TModel> EndingWith(string endMessage)
        {
            if (!_error.EndsWith(endMessage))
            {
                throw new ModelErrorAssertionException(string.Format("Expected error message for key '{0}' to end with '{1}', but instead found '{2}'.", _errorKey, endMessage, _error));
            }
            return _modelTest;
        }

        public ModelTest<TModel> Containing(string containsMessage)
        {
            if (!_error.Contains(containsMessage))
            {
                throw new ModelErrorAssertionException(string.Format("Expected error message for key '{0}' to contain '{1}', but instead found '{2}'.", _errorKey, containsMessage, _error));
            }
            return _modelTest;
        }

        public ModelErrorTest<TModel> AndModelErrorFor<TAttribute>(Expression<Func<TModel, TAttribute>> memberWithError)
        {
            return _modelTest.AndModelErrorFor(memberWithError);
        }

        public ModelErrorTest<TModel> AndModelError(string errorKey)
        {
            return _modelTest.AndModelError(errorKey);
        }
    }

    #endregion

    #region Exceptions

    public class ActionResultAssertionException : Exception
    {
        public ActionResultAssertionException(string message) : base(message) { }
    }

    public class ViewResultModelAssertionException : Exception
    {
        public ViewResultModelAssertionException(string message) : base(message) {}
    }

    public class ModelErrorAssertionException : Exception
    {
        public ModelErrorAssertionException(string message) : base(message) { }
    }

    #endregion
}
