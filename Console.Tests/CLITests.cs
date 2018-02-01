using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using Moq;
using Xunit;

namespace Console.Tests
{
    public class CLITests
    {
        [InlineData(new [] { "--path", "something" }, "something", 1, 0)]
        [InlineData(new string[] { }, null, 1, 0)]
        [InlineData(new [] { "--bogus" }, null, 0, 1)]
        [Theory]
        public void ArgsNoError_DispatchesDefaults(string[] args, string expectedPath, int noErrorTimes, int errorTimes) // do we have our methods paired up right?
        {
            var mTestCli = new Mock<CLI>();
            mTestCli.Setup(c => c.FillDefaultsAndBootstrap(It.IsAny<Options>()));
            mTestCli.Setup(c => c.HandleNotParsedAndBootstrap(It.IsAny<IEnumerable<CommandLine.Error>>()));

            mTestCli.Object.Run(args);

            mTestCli.Verify(c => c.FillDefaultsAndBootstrap(It.Is<Options>(o => o.Path == expectedPath)), Times.Exactly(noErrorTimes));
            mTestCli.Verify(c => c.HandleNotParsedAndBootstrap(It.IsAny<IEnumerable<CommandLine.Error>>()), Times.Exactly(errorTimes));
        }

        [InlineData("/tmp/somepath", "/tmp/somepath")]
        [InlineData(null, "C:\\temp\\")]
        [Theory]
        public void FillsDefaultsAndBootstraps(string pathVal, string expectedPath)
        {
            var bs = Mock.Of<IBootstrapper>();
            var cli = new CLI();
            cli.Bootstrapper = bs;

            cli.FillDefaultsAndBootstrap(new Options() { Path = pathVal });

            Mock.Get(bs).Verify(b => b.Bootstrap(It.Is<Options>(o => o.Path == expectedPath)), Times.Once);
        }

        [InlineData(new string[0], 1)]
        [InlineData(new string[] { "my message" }, 2)]
        [Theory]
        public void HandlesErrorsAndReportsToUser(string[] messages, int calls)
        {
            var write = Mock.Of<Action<string>>();
            var cli = new CLI();
            cli.DefaultOutputPath = "exercise it";
            cli.WriteConsole = write;

            var errors = messages.Select(m =>
            {
                var me = new Mock<Error>(ErrorType.MissingValueOptionError, false);
                me.Setup(e => e.ToString()).Returns(m);
                return me.Object;
            });
            cli.HandleNotParsedAndBootstrap(errors);

            Mock.Get(write).Verify(w => w(It.IsAny<string>()), Times.Exactly(calls));
        }
    }
}
