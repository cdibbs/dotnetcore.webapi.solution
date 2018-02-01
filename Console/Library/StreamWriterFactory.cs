using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Console
{
    public class StreamWriterFactory: IStreamWriterFactory
    {
        protected Func<string, StreamWriter> CreateTextFile;

        public StreamWriterFactory(Func<string, StreamWriter> ctm = null)
        {
            this.CreateTextFile = ctm ?? File.CreateText;
        }

        public TextWriter Overwrite(string path)
        {   
            return CreateTextFile(path);
        }
    }
}
