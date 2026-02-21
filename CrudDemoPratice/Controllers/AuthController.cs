using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrudDemoPratice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
           
        }

        //  Login API

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                // return result == null ? Unauthorized("Invalid credentials") : Ok(result);
                if (result == null)
                {
                  
                    return Unauthorized(new
                    {
                        status = 401,
                        message = "Invalid Username and password"
                    });
                }
                else
                {
                  
                    return Ok(new
                    {
                        status = 200,
                        message = "Login successfully",
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while processing login"
                });
            }
        }
        //  Signup API
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
         
            try
            {


                var result = await _authService.RegisterAsync(request);
                //  return result ? Ok("User registered successfully!") : Conflict("Username already exists");
                if (result)
                {
                 
                    return Ok(new
                    {
                        status = 200,
                        message = "User registered successfully!"
                    });
                }
                else
                {
                    return Conflict(new
                    {
                        status = 409,
                        message = "Username already exists"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An Error Occure while Processing Registration "
                });

            }
        }
    }


}
