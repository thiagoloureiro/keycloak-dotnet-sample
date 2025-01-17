﻿using KeycloakAdapter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KeycloakTry2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IConfiguration _configutation;

        KeycloakManager accessKeycloakData;
        public UserController(IConfiguration configutation)
        {
            accessKeycloakData = new KeycloakManager(configutation);
            _configutation = configutation;
        }



        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Post([FromBody] User accessUser)
        {
            try
            {
                IActionResult result = default;

                string jwt = Request.Headers["Authorization"];
                int statusCodeResult = accessKeycloakData.TryCreateUser(jwt, accessUser).Result;

                if (statusCodeResult == 201)
                {
                    HttpResponseObject<User> userCreated = accessKeycloakData.FindUserByEmail(jwt, accessUser.email).Result;
                    await accessKeycloakData.TryAddRole(jwt, userCreated.Object, "administrator");
                    result = Created(" ", userCreated.Object);
                }
                else
                {
                    result = new StatusCodeResult(statusCodeResult);
                }

                return result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult Put([FromBody] User accessUser)
        {
            try
            {
                IActionResult result = default;

                string jwt = Request.Headers["Authorization"];
                int statusCodeResult = accessKeycloakData.TryUpdateUser(jwt, accessUser).Result;

                result = new StatusCodeResult(statusCodeResult);

                return result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        [Authorize(Roles = "administrator")]
        public IActionResult Delete([FromBody] User accessUser)
        {
            try
            {
                IActionResult result = default;

                string jwt = Request.Headers["Authorization"];
                int statusCodeResult = accessKeycloakData.TryDeleteUser(jwt, accessUser).Result;

                result = new StatusCodeResult(statusCodeResult);

                return result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
