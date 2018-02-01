using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Autofac;
using CommandLine;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class Program
    {
        public static ICLI CLI { get; set; } = new CLI();

        public static void Main(string[] args)
        {
            /*
             * For the sake of IoC and unit testing, leave the static context, ASAP...
             */
            CLI.Run(args);
        }
    }
}