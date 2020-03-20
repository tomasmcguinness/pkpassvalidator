using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassValidator.API.Models
{
    public class Error
    {
        public Error(string name, string msg, int code)
        {
            ErrorName = name;
            Message = msg;
            StatusCode = code;
        }

        [JsonProperty("error")]
        public string ErrorName { get; set; }

        public string Message { get; set; }

        public int StatusCode { get; set; }
    }
}
