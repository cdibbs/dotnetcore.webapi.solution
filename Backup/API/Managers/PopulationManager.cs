using API.Authorization;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using AutoMapper;
using Data.Repositories.ReadOnly;
using Data.Repositories.ReadOnly.Utility;
using Data.Utilities;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace API.Managers
{
    public class PopulationManager : IPopulationManager
    {
        public IReadOnlyRepository Repo { get; set; }
        public IMapper Mapper { get; set; }
        public ILogger Logger { get; set; }
        public IAuthManager<V_Population> Auth { get; set; }
        public IPopulationSpecificationProvider Specs { get; set; }

        public PopulationManager(
            IReadOnlyRepository repo, IMapper mapper, ILogger logger, IAuthManager<V_Population> auth,
            IPopulationSpecificationProvider specs)
        {
            this.Repo = repo;
            this.Mapper = mapper;
            this.Logger = logger;
            this.Auth = auth;
            this.Specs = specs;
        }

        public Expression<Func<V_Population, object>>[] FilterIncludes { get; set; } = new Expression<Func<V_Population, object>>[0];
        public Expression<Func<V_Population, object>>[] GetIncludes { get; set; } = new Expression<Func<V_Population, object>>[0];
        public Dictionary<string, Expression<Func<V_Population, object>>> SortSelectors { get; set; }
            = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                        {"Last", t => t.Last},
                        {"First", t => t.First},
            };

        public IViewModel<V_Population, string>[] Filter(
            ISpecification<V_Population> spec,
            int page, int pageSize,
            SortSpecification[] sortSpecifications)
        {
            Auth.AuthorizeGet();
            Logger.Information($"Requesting filtered V_Populations: {spec.Metadata}");
            //var s = Expression.And((Expression<Func<T, bool>>)((T t) => spec(t)), (Expression<Func<T, bool>>)((T t) => Auth.GenerateFilterGet()(t)));
            var res = Repo.Page(spec.And(Auth.GenerateFilterGet()),
                new ReadOnlySortFactory<V_Population>(sortSpecifications, this.SortSelectors),
                page, pageSize,
                includes: FilterIncludes)
                .ToList();
            var resm = Mapper.Map<IEnumerable<PersonViewModel>>(res);
            Logger.Information($"Returning V_Populations, count: {resm?.Count()}."); // TODO rethink this.
            return resm.ToArray();
        }

        public IViewModel<V_Population, string> Get(string hawkId) => Get(Specs.PopulationByUsername<V_Population>(hawkId));
        public IViewModel<V_Population, string> Get(ISpecification<V_Population> spec)
        {
            Logger.Information($"Requesting V_Populations {spec.Metadata}.");
            Auth.AuthorizeGet();
            var res = Repo.FindOne(spec.And(Auth.GenerateFilterGet()), includes: GetIncludes);
            var resm = Mapper.Map<PersonViewModel>(res);
            Logger.Information($"Returning V_Populations {resm?.Id}.");
            return resm;
        }

        public IViewModel<V_Population, string> Add(BaseInputModel<V_Population> input)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_Population, string>[] AddMany(BaseInputModel<V_Population>[] input)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_Population, string> Delete(ISpecification<V_Population> spec)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_Population, string> Delete(string id)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_Population, string> Update(BaseInputModel<V_Population> input)
        {
            throw new NotImplementedException("View is read-only");
        }
    }
}