using AuthenticationMicroservice.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using AuthenticationMicroservice.Repository;
using Microsoft.Extensions.Logging;

namespace AuthenticationMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo authRepo;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepo authRepo, ILogger<AuthController> logger)
        {
            this.authRepo = authRepo;
            this._logger = logger;
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(LoginDTO dto)
        {
            try
            {
                _logger.LogInformation("SignIn Method Invoked , Username is " + dto.UserName);
                var response = await this.authRepo.Login(dto);
                if (response == null)
                {
                    _logger.LogWarning("Invalid Credential Attempted , UserName:{0} , Password: {1}", dto.UserName, dto.Password);
                    return Unauthorized("Invalid Credentials");
                }
                    
                _logger.LogInformation("Successfully logged In");
                return Ok(response);
            }
            catch(Exception e)
            {
                _logger.LogError("Error Occured , Error Message is " + e.Message );
                return BadRequest(e.Message);   
            }
        }
    }
}
