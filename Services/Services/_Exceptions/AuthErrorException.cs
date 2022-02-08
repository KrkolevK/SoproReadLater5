using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services._Exceptions
{
    public class AuthErrorException : Exception
    {
        public string ErrorMsg { get; }
        public AuthErrorException(string errorMsg)
        {
            ErrorMsg = errorMsg;
        }
    }
}
