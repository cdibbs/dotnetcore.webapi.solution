using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Console.Tests
{
    // TODO Could use some refactoring. Right now, it calls the real BuildConfiguration method.
    public class BootstrapperTests
    {
        [Fact]
        public void ResolvesKernelAndRuns()
        {
            var kernel = Mock.Of<IKernel>();
            var cont = new ContainerBuilder();
            cont.RegisterInstance(kernel).As<IKernel>();
            MockContainer.real = cont.Build();
            var b = new Bootstrapper<MockContainer>();
            var opts = new Options();

            b.Bootstrap(opts);

            Mock.Get(kernel).Verify(k => k.Go());
            Assert.True(MockContainer.hasRun);
        }
    }

    public class MockContainer : DIContainerBuilder
    {
        public static IContainer real { get; set; }
        public static bool hasRun { get; set; }
        public override IContainer Build(Options cmdLineOptions, IConfigurationRoot config)
        {
            hasRun = true;
            return real;
        }
    }
}
