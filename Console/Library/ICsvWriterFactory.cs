using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Console
{
    public interface ICsvWriterFactory
    {
        IWriter Create(TextWriter file, Configuration config);
    }
}
