using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Moq;
using Xunit;

namespace Console.Tests
{
    public class StreamWriterFactoryTests
    {
        // Regressiony sanity
        [Fact]
        public void DefaultMethodIsCreate()
        {
            var factory = new StreamWriterFactory();
            Func<string, StreamWriter> val = factory.GetType()
                .GetField("CreateTextFile", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(factory) as Func<string, StreamWriter>;
            Assert.Equal(File.CreateText, val);
        }

        [Fact]
        public void OverwriteCallsCreate()
        {
            var writer = Mock.Of<Func<string, StreamWriter>>();
            var factory = new StreamWriterFactory(writer);
            factory.Overwrite("some path");
            Mock.Get(writer).Verify(a => a(It.Is<string>(s => s == "some path")), Times.Once);
        }
    }
}
