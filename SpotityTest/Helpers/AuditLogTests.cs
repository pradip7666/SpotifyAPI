using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SpotifyAPI.Helpers;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotifyAPI.Tests.Helpers
{
    [TestClass]
    public class AuditLogTests
    {
        private List<LogEvent> _logEvents;

        [TestInitialize]
        public void SetUp()
        {
            _logEvents = new List<LogEvent>();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(new TestSink(logEvent => _logEvents.Add(logEvent)))
                .CreateLogger();
        }

        [TestMethod]
        public void LogInfo_ShouldLogInformation_WithProfilerStep()
        {
            // Arrange
            string message = "This is an info message";

            // Act
            AuditLog.LogInfo(message);

            // Assert
            Assert.AreEqual(1, _logEvents.Count);
            var logEvent = _logEvents.First();
            Assert.AreEqual(LogEventLevel.Information, logEvent.Level);
            Assert.AreEqual(message, logEvent.MessageTemplate.Text);
        }

        [TestMethod]
        public void LogError_ShouldLogError_WithProfilerStep()
        {
            // Arrange
            string message = "This is an error message";
            var exception = new Exception("Test exception");

            // Act
            AuditLog.LogError(message, exception);

            // Assert
            Assert.AreEqual(1, _logEvents.Count);
            var logEvent = _logEvents.First();
            Assert.AreEqual(LogEventLevel.Error, logEvent.Level);
            Assert.AreEqual(message, logEvent.MessageTemplate.Text);
            Assert.AreEqual(exception, logEvent.Exception);
        }

        [TestMethod]
        public void LogAudit_ShouldLogAuditInformation_WithProfilerStep()
        {
            // Arrange
            string message = "This is an audit message";

            // Act
            AuditLog.LogAudit(message);

            // Assert
            Assert.AreEqual(1, _logEvents.Count);
            var logEvent = _logEvents.First();
            Assert.AreEqual(LogEventLevel.Information, logEvent.Level);
            Assert.AreEqual("AUDIT: " + message, logEvent.MessageTemplate.Text);
        }

        private class TestSink : ILogEventSink
        {
            private readonly Action<LogEvent> _logEventAction;

            public TestSink(Action<LogEvent> logEventAction)
            {
                _logEventAction = logEventAction;
            }

            public void Emit(LogEvent logEvent)
            {
                _logEventAction(logEvent);
            }
        }
    }
}
