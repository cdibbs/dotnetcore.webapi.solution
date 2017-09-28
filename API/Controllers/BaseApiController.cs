using System;
using API.Managers;
using API.SpecificationProviders;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Data;

namespace API.Controllers
{
    public abstract class BaseApiController<T, TKey> : Controller
        where TKey: IComparable
        where T : IEntity<TKey>
    {
        public IBaseManager<T, TKey> Manager { get; set; }
        public virtual IBaseSpecificationProvider<T> Specs { get; set; }

        public BaseApiController(IBaseManager<T, TKey> manager)
        {
            this.Manager = manager;
        }
    }
}