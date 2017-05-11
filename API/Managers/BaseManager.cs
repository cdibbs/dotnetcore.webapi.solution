using API.Authorization;
using API.Models.InputModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using API.Validators;
using AutoMapper;
using Data;
using Data.Utilities;
using Newtonsoft.Json;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace API.Managers
{
    public class BaseManager<T> : IBaseManager<T, long> where T : BaseEntity
    {
        public IRepository Repo { get; set; }
        public IMapper Mapper { get; set; }
        public IValidator<T> Validator { get; set; }
        public ILogger Logger { get; set; }
        public IAuthManager<T> Auth { get; set; }
        public IBaseSpecificationProvider<T> Specs { get; set; }

        public BaseManager(IRepository repo, IMapper mapper, IValidator<T> validator, ILogger logger, IAuthManager<T> auth, IBaseSpecificationProvider<T> specs)
        {
            this.Repo = repo;
            this.Mapper = mapper;
            this.Validator = validator;
            this.Logger = logger;
            this.Auth = auth;
            this.Specs = specs;
        }

        public Expression<Func<T, object>>[] FilterIncludes { get; set; } = new Expression<Func<T, object>>[0];
        public Expression<Func<T, object>>[] GetIncludes { get; set; } = new Expression<Func<T, object>>[0];
        public Expression<Func<T, object>>[] DeleteIncludes { get; set; } = new Expression<Func<T, object>>[0];
        public Dictionary<string, dynamic> SortSelectors { get; set; }
            = new Dictionary<string, dynamic>()
            {
                {"Id", (Expression<Func<T, long>>)((T t) => t.Id) },
                {"Created", (Expression<Func<T, DateTime>>)((T t) => t.Created) },
                {"LastUpdated", (Expression<Func<T, DateTime>>)((T t) => t.LastUpdated) },
            };
        public bool UseHardDeletes = true;

        public virtual IViewModel<T, long>[] Filter(
            ISpecification<T> spec, int page, int pageSize, SortSpecification[] sortSpecs)
        {
            Auth.AuthorizeGet();
            Logger.Information($"Requesting filtered {typeof(T).FullName}s (@{page} of {pageSize}, {JsonConvert.SerializeObject(sortSpecs?.Select(s => s.ToString()))})");
            Logger.Information($"Filter: {JsonConvert.SerializeObject(spec.Metadata)}");
            //var s = Expression.And((Expression<Func<T, bool>>)((T t) => spec(t)), (Expression<Func<T, bool>>)((T t) => Auth.GenerateFilterGet()(t)));
            var sortFactory = new SortFactory<T>(sortSpecs, this.SortSelectors);
            var res = Repo
                .Page(spec.And(Auth.GenerateFilterGet()), page, pageSize,
                    sortFactory, includes:FilterIncludes, track: false)
                .ToList();
            var resm = Mapper.Map<IEnumerable<IViewModel<T>>>(res);
            Logger.Information($"Returning {typeof(T).FullName} count:{resm.Count()}.");
            return resm.ToArray();
        }

        public virtual IViewModel<T, long> Get(long id) => Get(Specs.ById<T>(id));

        /// <summary>
        /// Base unimplemented.
        /// </summary>
        /// <param name="id">An entity's string Id.</param>
        /// <returns>A view model of the entity.</returns>
        public virtual IViewModel<T, long> Get(string id)
        {
            throw new NotImplementedException("String ids not implemented.");
        }
        public virtual IViewModel<T, long> Get(ISpecification<T> spec)
        {
            Auth.AuthorizeGet();
            Logger.Information($"Get {typeof(T).FullName}: {JsonConvert.SerializeObject(spec.Metadata)}");
            var res = Repo.FindOne(spec.And(Auth.GenerateFilterGet()), includes:GetIncludes);
            var resm = Mapper.Map<IViewModel<T>>(res);
            Logger.Information($"Gotten {typeof(T).FullName} Id:{resm?.Id}");
            return resm;
        }

        /// <summary>
        /// Intended mainly for many-to-many join endpoints, this method
        /// allows you to submit many entities at once.
        /// </summary>
        /// <param name="inputs">New entities to create.</param>
        /// <returns>The saved entities (including their database ids).</returns>
        public virtual IViewModel<T, long>[] AddMany(BaseInputModel<T>[] inputs)
        {
            Auth.AuthorizeAdd();
            Logger.Information($"Add {typeof(T).FullName}");
            foreach (var input in inputs)
                Validator.Validate(input);
            var newObjs = Mapper.Map<T[]>(inputs);
            foreach(var obj in newObjs)
                Repo.AddEntity(obj);
            Repo.Save();
            Logger.Information($"Added {typeof(T).FullName} Ids:{newObjs.Select(n => n.Id).ToArray()}");
            return Mapper.Map<IViewModel<T>[]>(newObjs);
        }

        public virtual IViewModel<T, long> Add(BaseInputModel<T> input)
        {
            Auth.AuthorizeAdd();
            Logger.Information($"Add {typeof(T).FullName}");
            Validator.Validate(input);
            var newObj = Mapper.Map<T>(input);
            Repo.AddEntity(newObj);
            Repo.Save();
            Logger.Information($"Added {typeof(T).FullName} Id:{newObj?.Id}");
            return Mapper.Map<IViewModel<T>>(newObj);
        }

        // TODO what if target null?
        public virtual IViewModel<T, long> Update(BaseInputModel<T> input)
        {
            Auth.AuthorizeUpdate();
            Logger.Information($"Updating {typeof(T).FullName} Id:{input.Id}.");
            Validator.Validate(input);
            var existingObj = Repo.FindOne(Specs.ById<T>(input.Id).And(Auth.GenerateFilterUpdate()), track: true);
            Mapper.Map(input, existingObj);
            Repo.Save();
            Logger.Information($"Update of {typeof(T).FullName} Id:{input.Id} complete.");
            return Mapper.Map<IViewModel<T>>(existingObj);
        }

        /// <summary>
        /// Deletes the item with the given id and returns it. 
        /// If null is returned, no item was found.
        /// </summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <returns>The item deleted, or null, if not found.</returns>
        public virtual IViewModel<T, long> Delete(long id) => Delete(Specs.ById<T>(id));

        /// <summary>
        /// Deletes the first item specified by spec and returns it. 
        /// If null is returned, no item was found.
        /// </summary>
        /// <param name="spec">The specification of the item to delete.</param>
        /// <returns>The item deleted, or null, if not found.</returns>
        public virtual IViewModel<T, long> Delete(ISpecification<T> spec)
        {
            Auth.AuthorizeDelete();
            Logger.Information($"Deleting {typeof (T).FullName}.");
            Logger.Information($"Filter: {JsonConvert.SerializeObject(spec.Metadata)}");
            var result = Repo.Delete(spec.And(Auth.GenerateFilterDelete()), UseHardDeletes, DeleteIncludes);
            Repo.Save();
            Logger.Information($"Deleted, returning Id:{result?.Id}.");
            return Mapper.Map<IViewModel<T>>(result);
        }
    }
}