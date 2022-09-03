using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using AuthenticationMicroservice;
using AuthenticationMicroservice.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AuthenticationMicroservice.Repository
{
    public class AuthRepo : IAuthRepo
    {
       
        private readonly UserManager<AppUser> userManager;
        
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        public AuthRepo(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<LoginResponse> Login(LoginDTO dto)
        {
            AppUser user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null) return null;
            
            bool IsValidPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!IsValidPassword) return null;
            
            string key = configuration["JwtSettings:key"];
            string issuer = configuration["JwtSettings:issuer"];
            string audience = configuration["JwtSettings:audience"];
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            DateTime expires = DateTime.Now.AddMinutes(15);
            SecurityKey securityKey = new SymmetricSecurityKey(keyBytes);

            var userClaims = await userManager.GetClaimsAsync(user);
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));

            var userRoles = await userManager.GetRolesAsync(user);
            var role = userRoles.First();
            userClaims.Add(new Claim(ClaimTypes.Role, role));

            SigningCredentials credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: userClaims,
                signingCredentials: credentails,
                expires: expires);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string jwt = handler.WriteToken(token);

            var response = new LoginResponse() { UserName = user.UserName, Role = role, Token = jwt };
            return response;
        }
    }
}
