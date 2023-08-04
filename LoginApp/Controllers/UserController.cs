using LoginApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserContext _context;
        public UserController(IConfiguration config,UserContext context)
        {
            _config = config;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult Create(User user)
        {
            if(_context.Users.Where(u=>u.Email == user.Email).FirstOrDefault() != null)
            {
                return Ok("Already exists");
            }
            user.MemberSince = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Success");
        }
        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Pwd).FirstOrDefault();
            if(userAvailable != null)
            {
                return Ok(new JwtService(_config).GenerateToken(
                    userAvailable.UserId.ToString(),
                    userAvailable.FirstName,
                    userAvailable.LastName,
                    userAvailable.Email,
                    userAvailable.Mobile,
                    userAvailable.Gender
                    ));
            }
            else if (user.Email == "admin@gmail.com" && user.Pwd == "admin@123")
            {
                return Ok("admin");
            }
            return Ok("Failure");
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        
    }
}
