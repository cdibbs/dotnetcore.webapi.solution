using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Data;
using Data.Models;
using Data.Repositories.ReadOnly;
using Specifications;

namespace Console
{
    public class Kernel: IKernel
    {
        private IRepository Repo { get; set; }
        private ICsvWriterFactory CsvWriterFactory { get; set; }
        private IStreamWriterFactory StreamWriterFactory { get; set; }
        private Options Options { get; set; }

        public Kernel(
            IRepository repo,
            ICsvWriterFactory csvWriterFactory,
            IStreamWriterFactory streamWriterFactory,
            Options options)
        {
            this.Repo = repo;
            this.CsvWriterFactory = csvWriterFactory;
            this.StreamWriterFactory = streamWriterFactory;
            this.Options = options;
        }

        /**
         * The heart of your application truly begins, here.
         * Everything 'til now dealt with IoC setup, commandline args,
         * and the like.
         */
        public void Go()
        {
            // This template application just outputs a CSV from a
            // read-only data source (IDW?).

            var mappings = Repo.FindAll(Specification<User>.All());

            var csvConfig = new Configuration()
            {
                Delimiter = "|",
                QuoteAllFields = true
            };
            
            // csvConfig.RegisterClassMap<WriteYourCodeAndEatItToo>();

            var path = Path.Combine(Options.Path, "my.csv");
            using (var writer = StreamWriterFactory.Overwrite(path))
            {
                var csv = CsvWriterFactory.Create(writer, csvConfig);
                csv.WriteRecords(mappings);
            }
        }
    }
}
