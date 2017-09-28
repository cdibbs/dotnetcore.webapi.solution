using System;
using API.Models.InputModels;
using API.Models.ViewModels;
using Data;
using Data.Utilities;
using Specifications;

namespace API.Managers
{
    public interface IBaseManager<T, TKey>
        where TKey: IComparable
        where T: IEntity<TKey>
    {
        IViewModel<T, TKey>[] Filter(ISpecification<T> spec, int page, int pageSize,
            SortSpecification[] sortSpecifications);
        IViewModel<T, TKey> Get(TKey id);
        IViewModel<T, TKey> Get(ISpecification<T> spec);
        IViewModel<T, TKey> Add(BaseInputModel<T, TKey> input);

        /// <summary>
        /// Intended mainly for many-to-many join endpoints, this method
        /// allows you to submit many entities at once.
        /// </summary>
        /// <param name="inputs">New entities to create.</param>
        /// <returns>The saved entities (including their database ids).</returns>
        IViewModel<T, TKey>[] AddMany(BaseInputModel<T, TKey>[] inputs);

        IViewModel<T, TKey> Update(BaseInputModel<T, TKey> input);

        /// <summary>
        /// Deletes the item with the given id and returns it. 
        /// If null is returned, no item was found.
        /// </summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <returns>The item deleted, or null, if not found.</returns>
        IViewModel<T, TKey> Delete(TKey id);

        /// <summary>
        /// Deletes the first item specified by spec and returns it. 
        /// If null is returned, no item was found.
        /// </summary>
        /// <param name="spec">The specification of the item to delete.</param>
        /// <returns>The item deleted, or null, if not found.</returns>
        IViewModel<T, TKey> Delete(ISpecification<T> spec);
    }
}