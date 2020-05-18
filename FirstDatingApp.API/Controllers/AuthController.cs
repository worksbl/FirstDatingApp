using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FirstDatingApp.API.Data;
using FirstDatingApp.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FirstDatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var use = new List<int>() { 5, 6, 7 };
            return Ok(use);
        }

        // Previously string username, string password was used. Now we added a Dto - UserForRegisterDto which Maps the main
        // model (User class) to simpler objects used by view/API as for the view we only need Username and Password and No NEED of 
        // PasswordHash or PasswordSalt(that is in User Class). ViewModel in MVC is also for a similar purpose. Since here it is an API
        // we are using DTO as its only Data transfer and there is not View here

        // POST api/values
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegister)
        {
            userForRegister.Username = userForRegister.Username.ToLower();
            //if(! ModelState.IsValid)  -- Not required and even [FormBody] not required as we are using 
            //[Apicontroller] attribute at controller level return BadRequest("Model not valid");
            if (await _repo.UserExists(userForRegister.Username))
                return BadRequest("Username already Exists");
            var userToCreate = new User { Username = userForRegister.Username };
            var createdUser = await _repo.Register(userToCreate, userForRegister.Password);
            return StatusCode(201);

        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[] {
                new Claim (ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim (ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {token = tokenHandler.WriteToken(token)});


        }
    }
}