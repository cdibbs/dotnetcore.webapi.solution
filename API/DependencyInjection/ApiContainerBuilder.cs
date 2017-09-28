using API.Authorization;
using API.Managers;
using API.Mapping;
using API.SpecificationProviders;
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
using Data.Models;

namespace API.DependencyInjection
{
    public class ApiContainerBuilder: ContainerBuilder {
        private string _whoAmI;
        private IConfigurationRoot Configuration;

        public ApiContainerBuilder(string whoAmI) {
            _whoAmI = whoAmI;
        }

        public void Configure(IConfigurationRoot configuration) {
            this.Configuration = configuration;
            // We use property injection for primitive configuration values. It is typically quicker
            // for small-to-medium projects. If you end up injecting many config values into individual
            // classes, consider other design patterns around partitioning the values into subset classes.


            AppSettings();
            PseudoGlobals();
            Data();
            Specifications();
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
            this.Register(c => c.Resolve<IHttpContextAccessor>()?.HttpContext.User)
                .As<IPrincipal>();
            //this.RegisterInstance(c => null).As<IPrincipal>();
            //Bind<string>().ToConstant(RequestIP).InRequestScope().Named("RequestIP");
        }

        private void Data()
        {
            this.RegisterType<Repository>().As<IRepository>();
            this.RegisterType<DataContext>().As<IDataContext>();
            this.RegisterType<AllDataContext>().As<ISoftDeletedDataContext>();
            this.RegisterType<ReadOnlyDataContext>().As<IReadOnlyDataContext>();
            this.RegisterType<ReadOnlyRepository<string>>().As<IReadOnlyRepository<string>>();
        }

        private void Specifications()
        {
            this.RegisterType<PopulationSpecificationProvider>()
                .As<IPopulationSpecificationProvider>()
                .As<IBaseSpecificationProvider<V_MyView>>();

            this.RegisterType<UserSpecificationProvider>()
                .As<IUserSpecificationProvider>()
                .As<IBaseSpecificationProvider<User>>();

            this.RegisterType<RoleSpecificationProvider>()
                .As<IRoleSpecificationProvider>()
                .As<IBaseSpecificationProvider<Role>>();

            this.RegisterType<UserRoleSpecificationProvider>()
                .As<IUserRoleSpecificationProvider>()
                .As<IBaseSpecificationProvider<UserRole>>();
        }

        private void AuthorizationManagement()
        {
            this.RegisterType<UserAuthManager>().As<IAuthManager<User, long>>();
            this.RegisterType<UserRoleAuthManager>().As<IAuthManager<UserRole, long>>();
            this.RegisterType<RoleAuthManager>().As<IAuthManager<Role, long>>();
            this.RegisterType<PopulationAuthManager>().As<IAuthManager<V_MyView, string>>();
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
            this.RegisterType<PopulationManager>().As<IBaseManager<V_MyView, string>>();
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