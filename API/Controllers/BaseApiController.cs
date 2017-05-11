using API.Managers;
using API.SpecificationProviders;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers
{
    public abstract class BaseApiController<T, TKey> : Controller where T : IEntity
    {
        public IBaseManager<T, TKey> Manager { get; set; }
        public virtual IBaseSpecificationProvider<T> Specs { get; set; }

        public BaseApiController(IBaseManager<T, TKey> manager)
        {
            this.Manager = manager;
        }
    }
}