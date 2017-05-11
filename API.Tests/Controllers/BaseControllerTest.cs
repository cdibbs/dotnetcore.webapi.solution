using System;
using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using API.Controllers;
using Data;
using API.Managers;
using API.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.SpecificationProviders;

namespace API.Tests
{
    public abstract class BaseControllerTest<T, U, UKey> //: BaseApiController<U, UViewModel> 
        where T: BaseApiController<U, UKey>, new()
        where U: IEntity
    {
        protected MapperConfiguration MapperConfig { get; set; }
        protected string myUserId = "myid";

        protected ClaimsIdentity setupMockId()
        {
            var claim = new Claim("test", myUserId);
            var mockIdentity =
                Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            return mockIdentity;
        }

        protected virtual T GetController()
        {
            Assert.IsNotNull(MapperConfig, "You must setup MapperConfig in a TestInitialize or TestMethod.");
            var c = new T()
            {
                Manager = Mock.Of<IBaseManager<U, UKey>>(),
                Specs = Mock.Of<IBaseSpecificationProvider<U>>()
            };
            c.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = Mock.Of<ClaimsPrincipal>(ip => ip.Identity == setupMockId())
                }
            };
            return c;
        }
    }
}
