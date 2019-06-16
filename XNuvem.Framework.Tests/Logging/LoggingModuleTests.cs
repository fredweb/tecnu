using Autofac;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNuvem.Logging;
using ILogger = XNuvem.Logging.ILogger;
using ILoggerFactory = XNuvem.Logging.ILoggerFactory;
using NullLogger = XNuvem.Logging.NullLogger;

namespace XNuvem.Tests.Logging
{
    [TestFixture]
    public class LoggingModuleTests
    {
        [Test]
        public void LoggingModuleWillSetLoggerProperty() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var container = builder.Build();
            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);
        }

        [Test]
        public void LoggerFactoryIsPassedTheTypeOfTheContainingInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var stubFactory = new StubFactory();
            builder.RegisterInstance(stubFactory).As<ILoggerFactory>();

            var container = builder.Build();
            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);
            Assert.That(stubFactory.CalledType, Is.EqualTo(typeof(Thing)));
        }

        public class StubFactory : ILoggerFactory
        {
            public ILogger CreateLogger(Type type) {
                CalledType = type;
                return NullLogger.Instance;
            }

            public Type CalledType { get; set; }
        }

        [Test]
        public void DefaultLoggerConfigurationUsesLoggerFactoryOverTraceSource() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var container = builder.Build();

            log4net.Config.BasicConfigurator.Configure(new MemoryAppender());

            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);

            MemoryAppender.Messages.Clear();
            thing.Logger.Error("-boom{0}-", 42);
            Assert.That(MemoryAppender.Messages, Has.Some.Contains("-boom42-"));

            MemoryAppender.Messages.Clear();
            thing.Logger.Warning(new ApplicationException("problem"), "crash");
            Assert.That(MemoryAppender.Messages, Has.Some.Contains("problem"));
            Assert.That(MemoryAppender.Messages, Has.Some.Contains("crash"));
            Assert.That(MemoryAppender.Messages, Has.Some.Contains("ApplicationException"));
        }

        [Test]
        public void LoggerOnConstructor() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<AnotherThing>();
            var container = builder.Build();
            var thing = container.Resolve<AnotherThing>();
            Assert.That(thing.Logger, Is.Not.Null);
            Assert.That(NullLogger.Instance, Is.Not.Null);
        }
    }

    public class AnotherThing
    {
        public ILogger Logger { get; set; }

        public AnotherThing(ILoggerFactory loggerFactory) {
            var logger = loggerFactory.CreateLogger(this.GetType());
            Assert.That(logger, Is.Not.Null);
        }
    }

    public class Thing
    {
        public ILogger Logger { get; set; }
    }

    public class MemoryAppender : IAppender
    {
        static MemoryAppender() {
            Messages = new List<string>();
        }

        public static List<string> Messages { get; set; }

        public void DoAppend(LoggingEvent loggingEvent) {
            if (loggingEvent.ExceptionObject != null) {
                lock (Messages) Messages.Add(string.Format("{0} {1} {2}",
                    loggingEvent.ExceptionObject.GetType().Name,
                    loggingEvent.ExceptionObject.Message,
                    loggingEvent.RenderedMessage));
            }
            else lock (Messages) Messages.Add(loggingEvent.RenderedMessage);
        }

        public void Close() { }
        public string Name { get; set; }
    }
}
