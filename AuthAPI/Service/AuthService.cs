using AuthAPI.Data;
using AuthAPI.Models;
using AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(
                u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            if (user == null)
            {
                return new LoginResponseDto() { User = null, Token = "", Message = "Username is incorrect" };
            }
            
            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "", Message = "Password is invalid" };
            }

            //User found, generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);
            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await 
                    _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(
                        u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return string.Empty;
                }else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch(Exception ex)
            {
                //Log
            }
            return "Error encountered";
        }
    }
}
