using System;
using System.Collections.Generic;
using System.Text;
using NUnit;
using NUnit.Framework;
using AuthenticationMicroservice.Controllers;
using AuthenticationMicroservice.Model;
using System.Web.Http.Results;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using StatusCodeResult = Microsoft.AspNetCore.Mvc.StatusCodeResult;
using OkResult = Microsoft.AspNetCore.Mvc.OkResult;
using Microsoft.Extensions.Configuration;
//using IConfiguration = Castle.Core.Configuration.IConfiguration;
using System.Net.Sockets;
using AuthenticationMicroservice.Repository;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;

namespace AuthenticationMicroservice.tests
{
    [TestFixture]
    internal class AuthControllerShould
    {
        LoginDTO loginDTO;
        LoginResponse loginResponse;
        Mock<IConfiguration> configuration;
        Mock<IAuthRepo> authRepo;
        Mock<UserManager<AppUser>> userManager; 
        Mock<RoleManager<IdentityRole>> roleManager;
        

        [SetUp]
        public void Setup()
        {
            loginDTO = new LoginDTO()
            {
                UserName = "admin",
                Password = "Admin@123"
            };
            loginResponse = new LoginResponse()
            {
                UserName ="admin",
                Role = "Admin",
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkMDg3MTBhMi03NjY1LTRjNDUtYTE4Yy0yNDM1NDkxMjc2YzkiLCJzdWIiOiJhZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNjYxODUzMjE1LCJpc3MiOiJodHRwOi8vd3d3LmNvZ25pemFudC5jb20iLCJhdWQiOiJodHRwOi8vd3d3LmNvZ25pemFudC5jb20ifQ.lXbAsAMKZCfEGnHawv8jCAwxBxAtpqp6-7jMEPUa5es"
            };
            configuration = new Mock<IConfiguration>();
            authRepo = new Mock<IAuthRepo>();
            userManager = new Mock<UserManager<AppUser>>();
            roleManager = new Mock<RoleManager<IdentityRole>>();
        }

        

        [Test]
        public async Task  Signin_ValidUserWithRightAuthCredential_ReturnOk()
        {
            configuration.Setup(p => p["JwtSettings:key"]).Returns("sandyakkujayacts");
            configuration.Setup(p => p["JwtSettings:issuer"]).Returns("http://www.cognizant.com");
            configuration.Setup(p => p["JwtSettings:audience"]).Returns("http://www.cognizant.com");
            userManager.Setup(p => p.CreateAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243" }, "Admin@123")).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(p => p.AddToRoleAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243" }, "Admin")).ReturnsAsync(IdentityResult.Success);
            roleManager.Setup(p => p.CreateAsync(new IdentityRole { Name = "Admin" }));
            userManager.Setup(p => p.FindByNameAsync(loginDTO.UserName)).ReturnsAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243", PasswordHash = "AQAAAAEAACcQAAAAEI2/0YxCxG58p2VKONr8t9GFdUC9illJd1SN6CzKA4xEOE42QEFcBM0fRkYa1sFZlQ==" });
            userManager.Setup(p => p.CheckPasswordAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243", PasswordHash = "AQAAAAEAACcQAAAAEI2/0YxCxG58p2VKONr8t9GFdUC9illJd1SN6CzKA4xEOE42QEFcBM0fRkYa1sFZlQ==" }, loginDTO.Password)).ReturnsAsync(true);
            authRepo.Setup(s => s.Login(loginDTO)).ReturnsAsync(loginResponse);
            authRepo.Setup(s => s.Equals(It.IsAny<AppUser>())).Returns(Equals(true));
            userManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<AppUser, string>((x, y) => Is.EqualTo(1));
            var logger = Mock.Of<ILogger<AuthController>>();
            var res = new AuthController(authRepo.Object, logger);
            var data = res.SignIn(loginDTO);
            var dataStatusCode = await data as OkObjectResult;
            Assert.IsNotNull(data);
            Assert.AreEqual(200, dataStatusCode.StatusCode);
        }

        

        [Test]
        public async Task SignIn_InValidUserWithWrongAuthCredential_Return401()
        {
            configuration.Setup(p => p["JwtSettings:key"]).Returns("sandyakkujayacts");
            configuration.Setup(p => p["JwtSettings:issuer"]).Returns("http://www.cognizant.com");
            configuration.Setup(p => p["JwtSettings:audience"]).Returns("http://www.cognizant.com");
            userManager.Setup(p => p.CreateAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243" }, "Admin@123")).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(p => p.AddToRoleAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243" }, "Admin")).ReturnsAsync(IdentityResult.Success);
            authRepo.Setup(s => s.Login(loginDTO)).ReturnsAsync(loginResponse);
            authRepo.Setup(s => s.Equals(It.IsAny<AppUser>())).Returns(Equals(true));
            roleManager.Setup(p => p.CreateAsync(new IdentityRole { Name = "Admin" }));
            roleManager.Setup(p => p.RoleExistsAsync("Admin"));
            userManager.Setup(p => p.FindByNameAsync(loginDTO.UserName)).ReturnsAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243", PasswordHash = "AQAAAAEAACcQAAAAEI2/0YxCxG58p2VKONr8t9GFdUC9illJd1SN6CzKA4xEOE42QEFcBM0fRkYa1sFZlQ==" });
            userManager.Setup(p => p.CheckPasswordAsync(new AppUser() { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "9047277243", PasswordHash = "AQAAAAEAACcQAAAAEI2/0YxCxG58p2VKONr8t9GFdUC9illJd1SN6CzKA4xEOE42QEFcBM0fRkYa1sFZlQ==" }, loginDTO.Password)).ReturnsAsync(true);
            userManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<AppUser, string>((x, y) => Is.EqualTo(1));
            var logger = Mock.Of<ILogger<AuthController>>();
            var res = new AuthController(authRepo.Object, logger);
            var data = res.SignIn(new LoginDTO() { UserName = "admin123" , Password = "Navaneethan"});
            var dataStatusCode = await data as UnauthorizedObjectResult;
            
            Assert.IsNotNull(data);
            Assert.AreEqual(401, dataStatusCode.StatusCode);
            
        }
    }
}
