using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpotifyAPI.Filters;
using System.Collections.Generic;

namespace SpotifyAPI.Tests.Filters
{
    [TestClass]
    public class ValidateModelAttributeTests
    {
        private ValidateModelAttribute _attribute;

        [TestInitialize]
        public void SetUp()
        {
            _attribute = new ValidateModelAttribute();
        }

        [TestMethod]
        public void OnActionExecuting_ModelStateIsValid_ShouldNotSetResult()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(Mock.Of<HttpContext>(), new RouteData(), Mock.Of<ActionDescriptor>(), modelState);
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            // Act
            _attribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsNull(actionExecutingContext.Result);
        }

        [TestMethod]
        public void OnActionExecuting_ModelStateIsInvalid_ShouldSetBadRequestObjectResult()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Key", "ErrorMessage");

            var actionContext = new ActionContext(Mock.Of<HttpContext>(), new RouteData(), Mock.Of<ActionDescriptor>(), modelState);
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );

            // Act
            _attribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsNotNull(actionExecutingContext.Result);
            Assert.IsInstanceOfType(actionExecutingContext.Result, typeof(BadRequestObjectResult));
            var badRequestResult = actionExecutingContext.Result as BadRequestObjectResult;

            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
            var serializableError = badRequestResult.Value as SerializableError;
            Assert.IsTrue(serializableError.ContainsKey("Key"));
            Assert.AreEqual("ErrorMessage", ((string[])serializableError["Key"])[0]);
        }
    }
}
