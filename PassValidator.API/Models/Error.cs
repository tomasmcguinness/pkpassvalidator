using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassValidator.API.Models
{
    public class Error
    {
        public Error(string msg)
        {
            Message = msg;
        }

        public string Message { get; set; }
    }
}
