using System;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IAuthService
	{
		public Task<ResponseDto?> Login(LoginRequestDto loginRequestDto);
        public Task<ResponseDto?> Register(RegistrationRequestDto registrationRequestDto);
        public Task<ResponseDto?> AssignRole(RegistrationRequestDto registrationRequestDto);
    }
}

