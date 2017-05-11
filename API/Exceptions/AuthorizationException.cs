using System;

namespace API.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base()
        {
        }

        public AuthorizationException(string msg) : base(msg)
        {
        }
    }
}