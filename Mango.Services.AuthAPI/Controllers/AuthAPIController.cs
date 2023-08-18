using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {

        private readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMsg = await _authService.Register(registrationRequestDto);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                _response.IsSuccess = false;
                _response.Message = errorMsg;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            bool logResponse = await _authService.AssignRole(registrationRequestDto.Email,registrationRequestDto.Role.ToUpper());
            if (!logResponse)
            {
                _response.IsSuccess = false;
                _response.Message = "Wrong Role ";
                return BadRequest(_response);
            }
            _response.Result = logResponse;
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var logResponse = await _authService.Login(loginRequestDto);
            if (logResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Wrong Auth";
                return BadRequest(_response);
            }
            _response.Result = logResponse;
            return Ok(_response);
        }

    }
}

