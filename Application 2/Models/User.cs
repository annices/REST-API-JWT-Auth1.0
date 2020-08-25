using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace App2.Models
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
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("jwt")]
        public string JWT { get; set; }

        // Instantiate PasswordHasher from ASP.NET Core Identity:
        private readonly PasswordHasher<User> hasher = new PasswordHasher<User>();

        /// <summary>
        /// This function encrypts a user password with the HMAC-SHA256 algorithm
        /// based on the ASP.NET Core Identity framework.
        /// </summary>
        /// <param name="item>User object.</param>
        /// <returns>Hashed password.</returns>
        public string HashUserPassword(User item)
        {
            string hashedPassword = hasher.HashPassword(item, item.Password);
            return hashedPassword;
        }

        /// <summary>
        /// This function verifies a user password by comparing two hashed strings.
        /// </summary>
        /// <param name="item">User object.</param>
        /// <param name="hashedPassword">Hashed password.</param>
        /// <returns>True or false as in matched passwords or not.</returns>
        public bool VerifyUserPassword(User item, string passwordInput, string hashedPassword)
        {
            var result = hasher.VerifyHashedPassword(item, hashedPassword, passwordInput);

            if (result == PasswordVerificationResult.Success)
                return true;

            return false;
        }

    } // End class.
} // End namespace.
