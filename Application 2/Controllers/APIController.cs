using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App2.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/*
Copyright (c) 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
namespace App2.Controllers
{
    /// <summary>
    /// This App2 API controller enables the exchange of data between application 1 and application 2.
    /// </summary>
    public class APIController : ControllerBase
    {
        // Use IHttpClientFactory to enable http retries with Polly policy registered in Startup.cs:
        private readonly IHttpClientFactory _clientFactory;
        // Use IConfiguration to get necessary settings from appsettings.json:
        private readonly IConfiguration _iconfig;
        // Instantiate our database context:
        private readonly DBContext db = new DBContext();

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
        /// This is the start endpoint called once this application is launched. The start action can be changed in
        /// Startup.cs, and the launch URL can be managed in Properties > launchSettings.json.
        /// </summary>
        /// <returns>Start page.</returns>
        public async Task<string> Index() => "Hello world!";

        /// <summary>
        /// This endpoint receives user login credentials posted from application 1. The credentials are then
        /// verified against the App2 database with a response message sent back to application 1.
        /// </summary>
        /// <returns>JSON response message.</returns>
        public async Task<string> Login()
        {
            try
            {
                // Receive JSON content from the external app and assign it to a user object:
                User item = await HttpJsonContent();
                string emailInput = item.Email;
                string passwordInput = item.Password;
                // Then get the password stored in the App2 db:
                item = db.GetUser(_iconfig, emailInput);
                string dbPassword = item.Password;
                // Create the client we want to give our JSON message response to:
                _clientFactory.CreateClient(_iconfig[new APIClient().Name]);

                if (emailInput.Equals(item.Email) && new User().VerifyUserPassword(item, passwordInput, dbPassword))
                {
                    HttpContext.Response.StatusCode = 200; // 200 = OK.

                    Token token = new Token
                    {
                        JWT = GenerateJWT(item.Email),
                        Key = null,
                        Issuer = null,
                        Audience = null
                    };

                    // Respond with generated token in JSON format:
                    return await Task.Run(() => JsonConvert.SerializeObject(token));
                }
                else
                {
                    HttpContext.Response.StatusCode = 401; // 401 = Unauthorized.
                    string errorMsg = "{ 'Message': 'Unauthorized!' }";
                    return await Task.Run(() => JsonConvert.SerializeObject(errorMsg));
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This endpoint receives a request from the application 1 to get user details from the App2 database
        /// based on a user email. If the attached JWT is accepted, the user details are then sent
        /// back to application 1.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUser()
        {
            try
            {
                User item = await HttpJsonContent();
                _clientFactory.CreateClient(_iconfig[new APIClient().Name]);

                if (ValidateJWT(item.JWT))
                {
                    HttpContext.Response.StatusCode = 200;
                    item = db.GetUser(_iconfig, item.Email);
                    item.Password = null;
                    return await Task.Run(() => JsonConvert.SerializeObject(item));
                }
                else
                {
                    HttpContext.Response.StatusCode = 401;
                    string json = "{ 'Message': 'Invalid token!' }";
                    return await Task.Run(() => JsonConvert.SerializeObject(json));
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This endpoint receives a request from application 1 to update user details.
        /// If the attached JSON Web Token is valid, the request is granted and the
        /// user details are updated in the App2 database.
        /// </summary>
        /// <returns>JSON response message.</returns>
        public async Task<string> Update()
        {
            try
            {
                User item = await HttpJsonContent();
                _clientFactory.CreateClient(_iconfig[new APIClient().Name]);

                if (ValidateJWT(item.JWT))
                {
                    HttpContext.Response.StatusCode = 200;

                    if (!string.IsNullOrEmpty(item.Password))
                    {
                        string hashedPassword = new User().HashUserPassword(item);
                        item.Password = hashedPassword;
                    }
                    db.SaveUser(_iconfig, item);
                    return await Task.Run(() => JsonConvert.SerializeObject(item));
                }
                else
                {
                    HttpContext.Response.StatusCode = 401;
                    string json = "{ 'Message': 'Invalid token!' }";
                    return await Task.Run(() => JsonConvert.SerializeObject(json));
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This task gets a JSON message sent from an external application and then converts
        /// it into a C# object to be able to process its content further in this application.
        /// </summary>
        /// <returns>User object.</returns>
        private async Task<User> HttpJsonContent()
        {
            // Get a JSON message from an external application and read it into a string:
            StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            var httpContent = await reader.ReadToEndAsync();

            // Then convert and return the content as a C# object:
            return JsonConvert.DeserializeObject<User>(httpContent);
        }

        /// <summary>
        /// This function generates a JSON web token based on a specified key, issuer and audience from appsettings.json.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>JSON web token.</returns>
        private string GenerateJWT(string email)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig[new Token().Key]));
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, email),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _iconfig[new Token().Issuer],
                Audience = _iconfig[new Token().Audience],
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// This function validates a JSON web token based on a specified key, issuer and audience from appsettings.json.
        /// </summary>
        /// <param name="token">JSON web token.</param>
        /// <returns>True or false as in valid or invalid token.</returns>
        private bool ValidateJWT(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig[new Token().Key]));
            var myIssuer = _iconfig[new Token().Issuer];
            var myAudience = _iconfig[new Token().Audience];

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

    } // End class.
} // End namespace.
