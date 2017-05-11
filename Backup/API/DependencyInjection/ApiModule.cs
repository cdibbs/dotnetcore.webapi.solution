using API.Authorization;
using API.Managers;
using API.Mapping;
using API.Validators;
using Autofac;
using AutoMapper;
using Data;
using Data.Repositories.ReadOnly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;

namespace API.DependencyInjection
{
    public class ApiModule: ContainerBuilder {
        private string _whoAmI;
        private IConfigurationRoot Configuration;

        public ApiModule(string whoAmI) {
            _whoAmI = whoAmI;
        }

        public void Configure(IApplicationBuilder app, IConfigurationRoot configuration) {
            this.Configuration = configuration;
            // We use property injection for primitive configuration values. It is typically quicker
            // for small-to-medium projects. If you end up injecting many config values into individual
            // classes, consider other design patterns around partitioning the values into subset classes.


            AppSettings();
            PseudoGlobals();
            Data();
            AuthorizationManagement();
            InputValidation();
            EndpointManagement();
            Mapping();
        }

        private void PseudoGlobals()
        {
            this.Register(c => DateTime.Now).Named<DateTime>("Now");
            this.Register(c => DateTime.Today).Named<DateTime>("Today");
            this.Register(c => CreateLogger()).As<ILogger>();
            //this.RegisterInstance(c => null).As<IPrincipal>();
            //Bind<string>().ToConstant(RequestIP).InRequestScope().Named("RequestIP");
        }

        private void Data()
        {
            this.RegisterType<Repository>().As<IRepository>();
            this.RegisterType<DataContext>().As<IDataContext>();
            this.RegisterType<ReadOnlyDataContext>().As<IReadOnlyDataContext>();
            this.RegisterType<ReadOnlyRepository>().As<IReadOnlyRepository>();
        }

        private void AuthorizationManagement()
        {
            this.RegisterType<UserAuthManager>().As<IAuthManager<User>>();
            this.RegisterType<UserRoleAuthManager>().As<IAuthManager<UserRole>>();
            this.RegisterType<RoleAuthManager>().As<IAuthManager<Role>>();
            this.RegisterType<PopulationAuthManager>().As<IAuthManager<V_Population>>();
        }

        private void InputValidation()
        {
            this.RegisterType<UserValidator>().As<IValidator<User>>();
            this.RegisterType<RoleValidator>().As<IValidator<Role>>();
            this.RegisterType<UserRoleValidator>().As<IValidator<UserRole>>();
        }

        private void EndpointManagement()
        {
            this.RegisterType<UserManager>().As<IBaseManager<User, long>>();
            this.RegisterType<RoleManager>().As<IBaseManager<Role, long>>();
            this.RegisterType<PopulationManager>().As<IBaseManager<V_Population, long>>();
            this.RegisterType<UserRoleManager>().As<IBaseManager<UserRole, long>>();
        }

        private void Mapping() {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<APIMappingProfile>(); });
            this.Register(c => config.CreateMapper()).As<IMapper>();
        }

        private ILogger CreateLogger() {
            var result = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration)
                .CreateLogger();

            return result;
        }

        private void AppSettings() {
            new List<string>()
            {
                "AuthorizationAccessKey",
            }.ForEach(s => {
                this.Register(c => this.Configuration.GetValue<string>(s)).Named<string>(s);
            });
        }        
    }
}