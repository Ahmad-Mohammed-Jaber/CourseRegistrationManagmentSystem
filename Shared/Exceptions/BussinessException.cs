using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Exceptions
{
    public class BussinessException : Exception
    {
        public BussinessException(string message) : base(message)
        {

        }

        public BussinessException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
