using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Buffers;

namespace PassValidator.API.Formatters
{
    public class PascalCaseJsonProfileFormatter : JsonOutputFormatter
    {
        public PascalCaseJsonProfileFormatter()
            : base(new JsonSerializerSettings 
            { 
                ContractResolver = new DefaultContractResolver() 
            }, ArrayPool<char>.Shared)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json;profile=\"https://en.wikipedia.org/wiki/PascalCase\""));
        }
    }
}
