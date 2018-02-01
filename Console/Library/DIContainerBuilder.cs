using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using CsvHelper;
using Data;
using Data.Repositories.ReadOnly;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class DIContainerBuilder : ContainerBuilder
    {
        public virtual IContainer Build(Options cmdLineOptions, IConfigurationRoot config)
        {
            var connStr = config["ConfigurationStrings:Repository"];
            this.RegisterType<ReadOnlyRepository<string>>().As<IReadOnlyRepository<string>>();
            this.RegisterInstance(new ReadOnlyDataContext(connStr)).As<IReadOnlyDataContext>();
            this.RegisterType<Kernel>().As<IKernel>();
            this.RegisterType<CsvWriterFactory>().As<ICsvWriterFactory>();
            this.RegisterType<StreamWriterFactory>().As<IStreamWriterFactory>();
            this.RegisterInstance(cmdLineOptions).As<Options>();
            this.RegisterInstance(config).As<IConfiguration>();
            return Build();
        }
    }
}
