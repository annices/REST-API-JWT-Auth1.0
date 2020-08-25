using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVCApp1.Models;

namespace MVCApp1.Controllers
{
    /// <summary>
    /// This controller acts as an interaction layer between our API-, model- and view layer. That is, it posts and
    /// receives user data via our API and returns the results to be displayed for our end user in the GUI/views.
    /// </summary>
    public class HomeController : Controller
    {
        // Instantiate necessary configuration variables:
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _iconfig;

        /// <summary>
        /// This constructor injects necessary configuration dependencies to enable
        /// the functions we need once this controller is called.
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="iconfig"></param>
        public HomeController(IHttpClientFactory clientFactory, IConfiguration iconfig)
        {
            _clientFactory = clientFactory;
            _iconfig = iconfig;
        }

        /// <summary>
        /// This action renders the application start page.
        /// </summary>
        /// <returns>The start page.</returns>
        [HttpGet]
        public ActionResult Index() => View();

        /// <summary>
        /// This action renders the login page.
        /// <returns>The login page before submit.</returns>
        [HttpGet]
        public ActionResult Login() => View();

        /// <summary>
        /// This action renders the page when a user has requested to login, i.e. when the login form has been submitted.
        /// </summary>
        /// <param name="input">User object.</param>
        /// <returns>The login page after submit.</returns>
        [HttpPost]
        public async Task<ActionResult> Login(User input)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(input.Password))
            {
                Token token = await new APIController(_clientFactory, _iconfig).Login(input.Email, input.Password);

                // If a token is received, it means we have a valid login:
                if (token != null)
                {
                    // Then store the received JSON Web Token in a session to use for later update requests:
                    HttpContext.Session.SetString("token", token.JWT);

                    // Also, assign a session on the user email to sense the logged in user on other pages:
                    HttpContext.Session.SetString("email", input.Email);
                    TempData["email"] = HttpContext.Session.GetString("email");

                    // Finally, redirect the user to the update page with the user email attached as route value:
                    return RedirectToAction(nameof(Update), "Home", new { email = input.Email });
                }
                else
                {
                    TempData["feedback"] = "Invalid login.";
                    return View();
                }
            }
            return View();
        }

        /// <summary>
        /// This action logs out a user by clearing all active sessions.
        /// </summary>
        /// <returns>Login page.</returns>
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Remove("email");
            return RedirectToAction(nameof(Login));
        }

        /// <summary>
        /// This action renders the page to display the user update form with
        /// prepopulated values fetched from application 2 via our API.
        /// </summary>
        /// <param name="email">User email as input.</param>
        /// <returns>The update page before submit.</returns>
        [HttpGet]
        public async Task<ActionResult> Update(string email)
        {
            // Check if user is logged in:
            if (HttpContext.Session.GetString("email") != null)
            {
                User item = new User
                {
                    Email = email,
                    JWT = HttpContext.Session.GetString("token")
                };

                item = await new APIController(_clientFactory, _iconfig).GetUser(item.Email, item.JWT);

                if (item.ID == 0)
                    ViewBag.Error = "No user was found with that email.";

                return View(item);
            }
            return RedirectToAction(nameof(Login));
        }

        /// <summary>
        /// This action renders the page when a user has requested to be updated,
        /// i.e. when the update form has been submitted.
        /// </summary>
        /// <param name="item">User object.</param>
        /// <returns>The update page after submit.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(User item)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("email") != null)
                {
                    item.JWT = HttpContext.Session.GetString("token");

                    item = await new APIController(_clientFactory, _iconfig).Update(item);

                    if (item.ID == 0)
                        ViewBag.Error = "The user could not be updated.";
                    else
                        ViewBag.Success = "The user was updated successfully.";

                    return View(item);
                }
                return RedirectToAction(nameof(Login));
            }
            return View(item);
        }

    } // End class.
} // End namespace.