using Newtonsoft.Json;

namespace MVCApp1.Models
{
    /// <summary>
    /// This class specifies the JSON web token property.
    /// </summary>
    public class Token
    {
        [JsonProperty("jwt")]
        public string JWT { get; set; }

    } // End class.
} // End namespace.
