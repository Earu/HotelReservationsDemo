using HRD.Models;
using HRD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HRD.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> Logger;
        private readonly ISessionService Sessions;

        public AuthenticationController(ISessionService sessions, ILogger<AuthenticationController> logger)
        {
            this.Logger = logger;
            this.Sessions = sessions;
        }

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="login">The new user's info</param>
        /// <returns>The session token if registration was succesful</returns>
        [HttpPost]
        [Route("[controller]/SignUp")]
        public async Task<string> SignUp(UserLogin login)
        {
            this.Logger.LogInformation("/SignUp");

            try
            {
                return await this.Sessions.SignUp(login.Username, login.Password);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Attempts to sign in an existing user
        /// </summary>
        /// <param name="login">The user's info</param>
        /// <returns>The session token if sign in was succesful</returns>
        [HttpPost]
        [Route("[controller]/SignIn")]
        public async Task<string> SignIn(UserLogin login)
        {
            this.Logger.LogInformation("/SignIn");

            try
            {
                return await this.Sessions.SignIn(login.Username, login.Password);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                return null;
            }
        }
    }
}
