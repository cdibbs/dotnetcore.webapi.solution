using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Data;
using Data.Models;
using Data.Repositories.ReadOnly;
using Moq;
using Specifications;
using Xunit;

namespace Console.Tests
{
    public class KernelTests
    {
        private IRepository repo;
        private ICsvWriterFactory csvf;
        private IStreamWriterFactory swf;
        private Options opts;

        public KernelTests()
        {
            repo = Mock.Of<IRepository>();
            csvf = Mock.Of<ICsvWriterFactory>();
            swf = Mock.Of<IStreamWriterFactory>();
            opts = new Options();
        }

        [Fact]
        public void Constructs()
        {
            var kernel = new Kernel(repo, csvf, swf, opts);
        }

        [Fact]
        public void Go_FetchesAllAndWrites()
        {
            // Setup
            var repoReturn = Mock.Of<IQueryable<User>>();
            var writerReturn = new Mock<StreamWriter>("something").Object;
            var csvReturn = Mock.Of<IWriter>();
            opts.Path = "/tmp/mypath";
            Mock.Get(repo)
                .Setup(r => r.FindAll(It.IsAny<Specification<User>>(), false, false))
                .Returns(repoReturn);
            Mock.Get(swf)
                .Setup(s => s.Overwrite(It.Is<string>(p => p == "/tmp/mypath\\my.csv")))
                .Returns(writerReturn);
            Mock.Get(csvf)
                .Setup(s => s.Create(It.Is<StreamWriter>(sw => sw == writerReturn), It.IsAny<Configuration>()))
                .Returns(csvReturn);
            var kernel = new Kernel(repo, csvf, swf, opts);

            // Test
            kernel.Go();

            // Assert
            Mock.Get(repo).Verify(r => r.FindAll(It.IsAny<Specification<User>>(), false, false), Times.Once);
            Mock.Get(csvReturn).Verify(c => c.WriteRecords(It.Is<IQueryable<User>>(q => q == repoReturn)), Times.Once);
        }
    }
}
