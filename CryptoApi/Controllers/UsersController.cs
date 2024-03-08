using CryptoApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CryptoApi.Data.CryptoDbContext;

namespace CryptoApi.Controllers
{
    [Route("v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        CryptoDbContext _dbContext = new CryptoDbContext();
        private IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existedUser != null)
            {
                return BadRequest("User allready exists");
            }
            else
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return Ok("User created");
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existedUser = await _dbContext.Users.FirstOrDefaultAsync(u =>u.Email == user.Email);
            if (existedUser == null)
            {
                return BadRequest("User do not exist");
            }
            if(existedUser.Password != user.Password)
            {
                return BadRequest("Password is not valid");
            }
            else
            {
                var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim(ClaimTypes.Email, user.Email)
            };
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(jwt);
            }

        }
    }
}
