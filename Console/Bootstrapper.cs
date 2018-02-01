using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class Bootstrapper<TContainerBuilder>: IBootstrapper
        where TContainerBuilder: DIContainerBuilder, new()
    { 
        public void Bootstrap(Options options)
        {
            var config = BuildConfiguration();
            var cb = new TContainerBuilder();
            var container = cb.Build(options, config);
            using (var scope = container.BeginLifetimeScope())
            {
                scope.Resolve<IKernel>().Go();
            }
        }

        public virtual IConfigurationRoot BuildConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
