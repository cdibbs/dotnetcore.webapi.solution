using API.Exceptions;
using API.Models;
using Data;
using Serilog;
using Specifications;
using System.Security.Principal;

namespace API.Authorization
{
    public class BaseAuthManager<T> : IAuthManager<T> where T: IEntity
    {
        public IPrincipal User { get; set; }
        public ILogger Logger { get; set; }

        public string Requestor => $"{User?.Identity?.Name ?? "[Unknown]"}";
        private string Target => typeof (T).FullName;

        public virtual bool MayGet => User.IsInRole("ValidUser");
        public virtual bool MayAdd => User.IsInRole("ValidUser");
        public virtual bool MayUpdate => User.IsInRole("ValidUser");
        public virtual bool MayDelete => User.IsInRole("ValidUser");
        public virtual void AuthorizeGet()
        {
            if (! MayGet)
            {
                string message = $"Unauthorized Get {Target} access attempt by {Requestor}.";
                Logger.Warning(message);
                throw new AuthorizationException(message);
            }
        }

        public virtual ISpecification<T> GenerateFilterGet()
        {
            return Specification<T>.Start((T t) => MayGet);
        }

        public virtual void AuthorizeAdd() {
            if (! MayAdd)
            {
                throw new AuthorizationException($"Unauthorized Add {Target} access attempt by {Requestor}.");
            }
        }

        public virtual void AuthorizeUpdate()
        {
            if (! MayUpdate)
            {
                Logger.Warning($"Unauthorized Update {Target} access attempt by {Requestor}.");
                throw new AuthorizationException();
            }
        }
        public virtual ISpecification<T> GenerateFilterUpdate()
        {
            return Specification<T>.Start((T t) => MayUpdate);
        }

        public virtual void AuthorizeDelete() {
            if (! MayDelete)
            {
                Logger.Warning($"Unauthorized Delete {Target} access attempt by {Requestor}.");
                throw new AuthorizationException();
            }
        }
        public virtual ISpecification<T> GenerateFilterDelete()
        {
            return Specification<T>.Start(t => MayDelete);
        }

        protected bool IsInRole(RoleType role) => User?.IsInRole(role.ToString()) ?? false;
    }
}