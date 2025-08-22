using AutoMapper;
using HandiCraft.Application.DTOs.Services;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using HandiCraft.Infrastructure.EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HandiCraft.Presentation;

namespace HandiCraft.API.Controllers
{
    public class AccountController : APIControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthServices _AuthServics;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, IAuthServices authServics, IMapper mapper, IEmailService emailService)
        {
            _userManager = userManager;
            _env = env;
            _signInManager = signInManager;
            _AuthServics = authServics;
            _mapper = mapper;
            _emailService = emailService;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register([FromForm] RegisterDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new Response(400, "Email is already in use."));
                   
            }
            if (model.Password != model.ConfirmPassword)
                return BadRequest(new Response(400,"Passwords do not match."));

            string? profileImageUrl = null;

            if (model.ProfilePicture != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var path = Path.Combine(_env.WebRootPath, "images/users");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fullPath = Path.Combine(path, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                profileImageUrl = $"/images/users/{fileName}";
            }

            var user = new ApplicationUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = model.UserType,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                ProfilePicture = profileImageUrl
                
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var ReturnedObject = _mapper.Map<UserDto>(user);
            ReturnedObject.Token = await _AuthServics.CreateTokenAsync(user, _userManager);

            

            return Ok(new
            {
                message = "User registered successfully",
                ReturnedObject
            });
        }
        [HttpPost("Login")]
        public async Task<ActionResult>Login(LoginDto model)
        {
            var User=await _userManager.FindByEmailAsync(model.Email);
            if (User == null|| !await _userManager.CheckPasswordAsync(User, model.Password))
            {
                return Unauthorized(new Response(401,"Invalid email or password"));
            }
            var token = await _AuthServics.CreateTokenAsync(User, _userManager);
            return Ok(new
            {
                message = "User login successfuly",
                token = token
            });
        }
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserDto>>GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            if (userEmail == null)
            {

                return Unauthorized(new Response(401,"Invalid token or not authenticated."));
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new Response(400,"User not found."));

            var ReturnedObject = _mapper.Map<UserDto>(user);
            ReturnedObject.Token = await _AuthServics.CreateTokenAsync(user, _userManager);

            return Ok(ReturnedObject);
            
        }
        [Authorize]
        [HttpPut("updateProfile")]
        public async Task<ActionResult>UpdateProfile([FromForm] UpdateProfileDto model)
        {
            Console.WriteLine("UpdateProfile here");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new Response(400,"User not found"));
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.Gender))
                user.Gender = model.Gender;

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.UserName))
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                    return BadRequest(new Response(400,"Username already taken"));
                user.UserName = model.UserName;
            }
            if (model.DateOfBirth != null)
                user.DateOfBirth = model.DateOfBirth;

            if (model.ProfilePicture != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var folderPath = Path.Combine(_env.WebRootPath, "images/users");
                Directory.CreateDirectory(folderPath);
                var fullPath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                user.ProfilePicture = $"/images/users/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var ReturnedObject = _mapper.Map<UserDto>(user);
            ReturnedObject.Token = await _AuthServics.CreateTokenAsync(user, _userManager);

            return Ok(new
            {
                message = "Profile updated successfully",
                data = ReturnedObject
            });


        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new Response(400, "Email not found."));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var body = $@"
                <p>Hello,</p>
                <p>You requested to reset your password.</p>
                <p>Your reset token is:</p>
                <h3>{token}</h3>
                <p>Use this token in the app to reset your password.</p>
            ";

            await _emailService.SendEmailAsync(
                to: model.Email,
                subject: "Reset Your Password",
                body: body
            );

            return Ok(new
            {
                message = "Password reset token has been sent to your email."
            });
        }



        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest(new Response(400,"Passwords do not match."));

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new Response(400,"Invalid email."));

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully." });
        }
        [HttpGet("AllUsers")]
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }
    }
}
