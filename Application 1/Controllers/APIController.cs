using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCApp1.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System; 

namespace MVCApp1.Controllers
{
    /// <summary>
    /// This App1 API controller enables the exchange of data between application 1 and application 2.
    /// </summary>
    public class APIController : Controller
    {
        // Use IHttpClientFactory to enable http retries with Polly policy registered in Startup.cs:
        private readonly IHttpClientFactory _clientFactory;
        // Use IConfiguration to get necessary settings from appsettings.json:
        private readonly IConfiguration _iconfig;

        /// <summary>
        /// This constructor injects necessary configuration dependencies to enable
        /// the functions we need once this API controller is called.
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="iconfig"></param>
        public APIController(IHttpClientFactory clientFactory, IConfiguration iconfig)
        {
            _clientFactory = clientFactory;
            _iconfig = iconfig;
        }

        /// <summary>
        /// This endpoint receives the login credentials that have been submitted from the login page.
        /// The credentials are then posted to application 2, which will verify the login with
        /// a response message sent back to our application 1.
        /// </summary>
        /// <param name="userEmail">User email as input.</param>
        /// <param name="userPassword">User password as input.</param>
        /// <returns>User object.</returns>
        public async Task<Token> Login(string userEmail, string userPassword)
        {
            User item = new User
            {
                Email = userEmail,
                Password = userPassword
            };

            var result = await PostHttpJsonContent(item, "/API/Login");
            var httpContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Token>(CleanJsonFormat(httpContent));

            return null;
        }

        /// <summary>
        /// This endpoint calls an API url/endpoint in application 2 to request user details. The application 2
        /// then validates the request based on an attached JWT and returns a response message accordingly.
        /// </summary>
        /// <param name="userEmail">User email as input.</param>
        /// <param name="jwt">JSON web token as input.</param>
        /// <returns></returns>
        public async Task<User> GetUser(string userEmail, string jwt)
        {
            User item = new User
            {
                Email = userEmail,
                JWT = jwt
            };

            var result = await PostHttpJsonContent(item, "/API/GetUser");
            var httpContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<User>(CleanJsonFormat(httpContent));

            return null;
        }

        /// <summary>
        /// This endpoint calls an API url/endpoint in application 2 to update user details. The application 2
        /// then validates the request based on an attached JWT and returns a response message accordingly.
        /// </summary>
        /// <param name="input">User object.</param>
        /// <returns>Updated user data if token is granted, otherwise unknown.</returns>
        public async Task<User> Update(User input)
        {
            var result = await PostHttpJsonContent(input, "/API/Update");
            var httpContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<User>(CleanJsonFormat(httpContent));

            return null;
        }

        /// <summary>
        /// This task converts a C# object into a JSON object, and then posts
        /// and receives the object data via a specific API url/endpoint.
        /// </summary>
        /// <param name="item">C# object.</param>
        /// <param name="endpoint">API url.</param>
        /// <returns>The result received from the post request.</returns>
        private async Task<HttpResponseMessage> PostHttpJsonContent(Object model, string endpoint)
        {
            var client = _clientFactory.CreateClient(_iconfig[new APIClient().Name]);

            string json = await Task.Run(() => JsonConvert.SerializeObject(model));
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(endpoint, httpContent);

            return result;
        }

        /// <summary>
        /// This function cleans up a dirty JSON message format by getting rid of extra back slashes
        /// before it can be properly converted into a C# object and used further in this application.
        /// </summary>
        /// <param name="content">The JSON message.</param>
        /// <returns>Clean JSON format.</returns>
        private string CleanJsonFormat(string content)
        {
            string response = content.Replace(@"\", "");
            response = response.Remove(0, 1);
            response = response.Remove(response.Length - 1);

            return response;
        }

    } // End class.
} // End namespace.