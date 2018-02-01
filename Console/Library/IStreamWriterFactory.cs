using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Console
{
    public interface IStreamWriterFactory
    {
        TextWriter Overwrite(string path);
    }
}
