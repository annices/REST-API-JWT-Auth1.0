using Newtonsoft.Json;

namespace App2.Models
{
    /// <summary>
    /// This class specifies the properties used by the JSON Web Token.
    /// </summary>
    public class Token
    {
        // Set default values based on defined properties in appsettings.json:
        private string _key = "Jwt:Key";
        private string _issuer = "Jwt:Issuer";
        private string _audience = "Jwt:Audience";

        [JsonProperty("jwt")]
        public string JWT { get; set; }

        [JsonProperty("key")]
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        [JsonProperty("issuer")]
        public string Issuer
        {
            get => _issuer;
            set => _issuer = value;
        }

        [JsonProperty("audience")]
        public string Audience
        {
            get => _audience;
            set => _audience = value;
        }

    } // End class.
} // End namespace.
