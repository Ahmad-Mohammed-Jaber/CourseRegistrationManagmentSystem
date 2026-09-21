using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
