using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Implementation
{
    public class AuthService:IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _config;

        public AuthService(IUserRepository repo , IConfiguration conf)
        {
            _userRepository = repo;
            _config = conf;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request) {

            try {

                var user = await _userRepository.GetUserByCredentialsAsync(request.Username, request.Password);
                if (user == null) {

                    return null;
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role,user.Role)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                 issuer: _config["Jwt:Issuer"],
                 audience: _config["Jwt:Audience"],
                 claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(15),
                 signingCredentials: creds
             );

                return new LoginResponse{ 
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Role = user.Role
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        
        }


        public async Task<bool> RegisterAsync(RegisterRequest request) {

            try
            {

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Password = request.Password, // In production, hash the password!
                    Role = "User" // Default role
                };


                var result = await _userRepository.RegisterUserAsync(user);

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
            

        }

    }
}
