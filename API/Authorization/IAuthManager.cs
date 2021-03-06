﻿using System;
using Data;
using Specifications;

namespace API.Authorization
{
    public interface IAuthManager<T, TKey>
        where TKey: IComparable
        where T: IEntity<TKey>
    {
        bool MayGet { get; }
        bool MayAdd { get; }
        bool MayUpdate { get; }
        bool MayDelete { get; }

        /// <summary>
        /// Throws an AuthorizationException if the current user
        /// is not authorized for any GET operations.
        /// </summary>
        void AuthorizeGet();
        void AuthorizeAdd();
        void AuthorizeUpdate();
        void AuthorizeDelete();
        ISpecification<T> GenerateFilterGet();
        ISpecification<T> GenerateFilterUpdate();
        ISpecification<T> GenerateFilterDelete();
    }
}