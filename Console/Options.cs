using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Console
{
    public class Options
    {
        [Option('p', "path", Required = false, HelpText = "Base path for output files")]
        public string Path { get; set; }
    }
}
