using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using CommandLine;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class CLI : ICLI
    {
        public string DefaultOutputPath { get; set; } = "C:\\temp\\";
        public IBootstrapper Bootstrapper { get; set; } = new Bootstrapper<DIContainerBuilder>();
        public Action<string> WriteConsole { get; set; } = System.Console.WriteLine;

        public void Run(string[] args)
        {
            ParserResult<Options> parseResult = Parser.Default.ParseArguments<Options>(args);
            parseResult
                .WithNotParsed(HandleNotParsedAndBootstrap)
                .WithParsed(FillDefaultsAndBootstrap);
        }

        public virtual void FillDefaultsAndBootstrap(Options options)
        {
            options.Path = options.Path ?? DefaultOutputPath;
            Bootstrapper.Bootstrap(options);
        }

        public virtual void HandleNotParsedAndBootstrap(IEnumerable<Error> errors)
        {
            WriteConsole("Sorry, I didn't understand the supplied parameters:");
            foreach (var error in errors)
            {
                WriteConsole(error.ToString());
            }
        }
    }
}
