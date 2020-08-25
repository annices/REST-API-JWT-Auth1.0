using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MVCApp1.Models
{
    /// <summary>
    /// This class specifies properties used to create or update user details.
    /// </summary>
    public class User
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("jwt")]
        public string JWT { get; set; }

    } // End class.
} // End namespace.
