using System;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
	public class AuthService : IAuthService
	{
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

		public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator)
		{
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
		}

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var _user = _db.ApplicationUsers.FirstOrDefault(o => o.UserName.ToLower() == email.ToLower());
            if (_user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(_user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var _user = _db.ApplicationUsers.FirstOrDefault(o => o.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(_user, loginRequestDto.Password);
            if (_user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };

            }

            UserDto userDto = new()
            {
                Email = _user.Email,
                ID = _user.Id,
                Name = _user.Name,
                PhoneNumber = _user.PhoneNumber
            };
            var roles = await _userManager.GetRolesAsync(_user);
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = _jwtTokenGenerator.GenerateToken(_user,roles)
            };

            return loginResponseDto;
        }

        

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName=registrationRequestDto.Email,
                Email=registrationRequestDto.Email,
                Name=registrationRequestDto.Name,
                NormalizedEmail=registrationRequestDto.Email.ToUpper(),
                PhoneNumber=registrationRequestDto.PhoneNumber
            };
            try
            {
                var result =await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if(result.Succeeded)
                {
                    var userret = _db.ApplicationUsers.First(o => o.UserName == registrationRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userret.Email,
                        ID = userret.Id,
                        Name = userret.Name,
                        PhoneNumber = userret.PhoneNumber
                    };
                    return "";

                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }catch(Exception e)
            {
                return e.InnerException.Message;
            }
           // return "Error COY";
        }
    }
}

