﻿using System.Threading.Tasks;
using Lab7.ViewModels.Authentication;
using Microsoft.AspNetCore.Mvc;
using Lab7.Services;

namespace Lab7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
		private IAuthManagementService _authenticationService;

		public AuthenticationController(IAuthManagementService authService)
        {
            _authenticationService = authService;
        }

        [HttpPost]
        [Route("register")] // /api/authentication/register
        public async Task<ActionResult> RegisterUser(RegisterRequest registerRequest)
        {
            var registerServiceResult = await _authenticationService.RegisterUser(registerRequest);
            if (registerServiceResult.ResponseError != null)
            {
                return BadRequest(registerServiceResult.ResponseError);
            }

            return Ok(registerServiceResult.ResponseOk);
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<ActionResult> ConfirmUser(ConfirmUserRequest confirmUserRequest)
        {
            var serviceResult = await _authenticationService.ConfirmUserRequest(confirmUserRequest);
            if (serviceResult)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginRequest loginRequest)
        {
            var serviceResult = await _authenticationService.LoginUser(loginRequest);
            if (serviceResult.ResponseOk != null)
            {
                return Ok(serviceResult.ResponseOk);
            }

            return Unauthorized();
        }
    }
}