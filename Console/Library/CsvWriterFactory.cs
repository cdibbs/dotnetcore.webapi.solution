using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Console
{
    public class CsvWriterFactory : ICsvWriterFactory
    {
        public IWriter Create(TextWriter file, Configuration config)
        {
            return new CsvWriter(file, config);
        }
    }
}
