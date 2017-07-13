using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB
{
    public class ToDBException : ApplicationException
    {
        public ToDBException(string message) : base(message) { }
        public ToDBException() : base() { }
    }
}
