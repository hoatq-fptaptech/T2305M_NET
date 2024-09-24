using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using T2305M_API.Entities;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using T2305M_API.DTO.RequestModel.Auth;
using BCrypt.Net;
using T2305M_API.DTO.ResponseModel.Auth;
// JWT AUTH
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace T2305M_API.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : Controller
    {
        private readonly T2305mApiContext _context;
        private readonly IConfiguration _config;

        public AuthController(T2305mApiContext context,
            IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        private string GenJWT(User user)
        {
            string key = _config["JWT:Key"];
            string issuerX = _config["JWT:Issuer"];
            string audienceX = _config["JWT:Audience"];
            int lifeTime = Convert.ToInt32(_config["JWT:Lifetime"]);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signatureKey = new SigningCredentials(secretKey,
                            SecurityAlgorithms.HmacSha256);
            var payload = new[] {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.Fullname),
                new Claim("Role",user.Role)
            };

            var token = new JwtSecurityToken(
                    issuer: issuerX,
                    audience: audienceX,
                    payload,
                    expires: DateTime.Now.AddMinutes(lifeTime),
                    signingCredentials: signatureKey
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Please input user information");
                string salt = BCrypt.Net.BCrypt.GenerateSalt(6);
                string pwd_hashed = BCrypt.Net.BCrypt.HashPassword(model.password, salt);
                User user = new Entities.User
                    {
                        Email = model.email,
                        Fullname = model.fullname,
                        Password = pwd_hashed
                    };
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok(new AuthResponse
                {
                    jwt = GenJWT(user),
                    expired_at = DateTime.Now.AddMinutes(60)
                });
            }catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Email or password is not correct!");
                User user = _context.Users.Where(
                        u => u.Email.Equals(model.email)).First();
                if(user == null)
                    throw new Exception("Email or password is not correct!");
                bool verified = BCrypt.Net.BCrypt.Verify(model.password, user.Password);
                if(!verified)
                    throw new Exception("Email or password is not correct!");
                // login successfully
                return Ok(new AuthResponse
                {
                    jwt = GenJWT(user),
                    expired_at = DateTime.Now.AddMinutes(60)
                });
            }
            catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
    }

    
}

