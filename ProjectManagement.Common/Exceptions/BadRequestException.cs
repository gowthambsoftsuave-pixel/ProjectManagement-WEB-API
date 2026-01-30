using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
