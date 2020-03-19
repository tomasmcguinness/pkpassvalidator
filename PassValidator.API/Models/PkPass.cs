using Newtonsoft.Json;

namespace PassValidator.API.Models
{
    public class PkPass
    {
        [JsonProperty("encoded_bytes")]
        public string EncodedBytes { get; set; }
    }
}
