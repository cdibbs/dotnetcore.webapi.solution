using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;

namespace Console.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void CallsCLIWithArgs()
        {
            var args = new string[0];
            Program.CLI = Mock.Of<ICLI>();

            Program.Main(args);

            Mock.Get(Program.CLI)
                .Verify(c => c.Run(It.Is<string[]>(s => s == args)), Times.Once);
        }
    }
}
