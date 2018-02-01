using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Console.Tests
{
    public class DIContainerBuilderTests
    {
        [Fact]
        public void ConstructsAndBuilds()
        {
            var mopts = Mock.Of<Options>();
            var mconf = Mock.Of<IConfigurationRoot>();
            var di = new DIContainerBuilder();
            Assert.NotNull(di.Build(mopts, mconf));
        }
    }
}
