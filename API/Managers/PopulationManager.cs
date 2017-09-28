using API.Authorization;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using AutoMapper;
using Data.Repositories.ReadOnly;
using Data.Utilities;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Data.Models;

namespace API.Managers
{
    public class PopulationManager : IPopulationManager, IBaseManager<V_MyView, string>
    {
        public IReadOnlyRepository<string> Repo { get; set; }
        public IMapper Mapper { get; set; }
        public ILogger Logger { get; set; }
        public IAuthManager<V_MyView, string> Auth { get; set; }
        public IPopulationSpecificationProvider Specs { get; set; }

        public PopulationManager(
            IReadOnlyRepository<string> repo, IMapper mapper, ILogger logger, IAuthManager<V_MyView, string> auth,
            IPopulationSpecificationProvider specs)
        {
            this.Repo = repo;
            this.Mapper = mapper;
            this.Logger = logger;
            this.Auth = auth;
            this.Specs = specs;
        }

        public Expression<Func<V_MyView, object>>[] FilterIncludes { get; set; } = new Expression<Func<V_MyView, object>>[0];
        public Expression<Func<V_MyView, object>>[] GetIncludes { get; set; } = new Expression<Func<V_MyView, object>>[0];
        public Dictionary<string, dynamic> SortSelectors { get; set; }
            = new Dictionary<string, dynamic>()
            {
                {"Username", (Expression<Func<V_MyView, string>>)(t => t.Id)},
            };

        public IViewModel<V_MyView, string>[] Filter(
            ISpecification<V_MyView> spec,
            int page, int pageSize,
            SortSpecification[] sortSpecifications)
        {
            Auth.AuthorizeGet();
            Logger.Information($"Requesting filtered V_Populations: {spec.Metadata}");
            //var s = Expression.And((Expression<Func<T, bool>>)((T t) => spec(t)), (Expression<Func<T, bool>>)((T t) => Auth.GenerateFilterGet()(t)));
            var res = Repo.Page(spec.And(Auth.GenerateFilterGet()),
                new SortFactory<V_MyView, string>(sortSpecifications, this.SortSelectors),
                page, pageSize,
                includes: FilterIncludes)
                .ToList();
            var resm = Mapper.Map<IEnumerable<PersonViewModel>>(res);
            Logger.Information($"Returning V_Populations, count: {resm?.Count()}."); // TODO rethink this.
            return resm.ToArray();
        }

        public IViewModel<V_MyView, string> Get(string username) => Get(Specs.ByUserId<V_MyView>(username));
        public IViewModel<V_MyView, string> Get(ISpecification<V_MyView> spec)
        {
            Logger.Information($"Requesting V_Populations {spec.Metadata}.");
            Auth.AuthorizeGet();
            var res = Repo.FindOne(spec.And(Auth.GenerateFilterGet()), includes: GetIncludes);
            var resm = Mapper.Map<PersonViewModel>(res);
            Logger.Information($"Returning V_Populations {resm?.Id}.");
            return resm;
        }

        public IViewModel<V_MyView, string> Add(BaseInputModel<V_MyView, string> input)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_MyView, string>[] AddMany(BaseInputModel<V_MyView, string>[] input)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_MyView, string> Delete(ISpecification<V_MyView> spec)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_MyView, string> Delete(string id)
        {
            throw new NotImplementedException("View is read-only");
        }

        public IViewModel<V_MyView, string> Update(BaseInputModel<V_MyView, string> input)
        {
            throw new NotImplementedException("View is read-only");
        }
    }
}