using System;
using System.IO;
using CsvHelper.Configuration;
using Moq;
using Xunit;

namespace Console.Tests
{
    public class CsvWriterFactoryTests
    {
        [Fact]
        public void Create_BuildsInstance()
        {
            var f = new CsvWriterFactory();
            var config = new Configuration();
            var tw = Mock.Of<TextWriter>();

            var instance = f.Create(tw, config);

            Assert.NotNull(instance);
        }
    }
}
