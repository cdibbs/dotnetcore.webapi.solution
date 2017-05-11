using Data;
using Specifications;

namespace API.SpecificationProviders
{
    public interface IBaseSpecificationProvider<T>
    {
        ISpecification<T> All();
        ISpecification<T> ById<T>(long id) where T: IBaseEntity;
        ISpecification<T> None();
    }
}