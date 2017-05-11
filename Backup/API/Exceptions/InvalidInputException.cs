using System;

namespace API.Exceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException() : base()
        {
        }

        public InvalidInputException(string msg) : base(msg)
        {
        }
    }
}