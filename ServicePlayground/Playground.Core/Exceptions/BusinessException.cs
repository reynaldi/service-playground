using System;
using System.Collections.Generic;

namespace Playground.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ICollection<string> ErrorMessages { get; set; }

        public BusinessException() : base() { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(ICollection<string> messages)
        {
            ErrorMessages = messages;
        }
    }
}
