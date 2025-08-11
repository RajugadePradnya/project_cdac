using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidReachApi.Data;
using RapidReachApi.Helpers;
using RapidReachApi.Models;
using RapidReachApi.Services;
using System;
using System.Threading.Tasks;

namespace RapidReachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RapidReachDbContext _dbContext;
        private readonly IEmailService _emailService; // You'll implement this

        public AuthController(RapidReachDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Email already exists.");

            var salt = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPassword(model.Password, salt);

            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                PasswordHash = hashedPassword,
                PasswordSalt = Convert.ToBase64String(salt),
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Registration successful." });
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var saltBytes = Convert.FromBase64String(user.PasswordSalt);
            var hashedInputPassword = PasswordHelper.HashPassword(model.Password, saltBytes);

            if (hashedInputPassword != user.PasswordHash)
                return Unauthorized("Invalid email or password");

            return Ok(new { message = "Login successful." }); // TODO: Generate JWT
        }

        // STEP 1 - REQUEST PASSWORD RESET
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ResetRequestModel model)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return NotFound(new { error = "User not found" });

            var token = Guid.NewGuid().ToString();
            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            _dbContext.PasswordResetTokens.Add(resetToken);
            await _dbContext.SaveChangesAsync();

            var resetLink = $"http://localhost:3000/reset-password?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Reset Your Password",
                $"Click the link to reset your password: {resetLink}");

            return Ok(new { message = "Password reset link sent to your email." });
        }

        // STEP 2 - VERIFY RESET TOKEN
        [HttpGet("verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromQuery] string token)
        {
            var resetRecord = await _dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (resetRecord == null || resetRecord.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { error = "Invalid or expired token" });

            return Ok(new { message = "Token is valid" });
        }

        // STEP 3 - RESET PASSWORD
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var resetRecord = await _dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == model.Token);
            if (resetRecord == null || resetRecord.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { error = "Invalid or expired token" });

            var user = await _dbContext.Users.FindAsync(resetRecord.UserId);
            if (user == null) return NotFound(new { error = "User not found" });

            var saltBytes = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPassword(model.NewPassword, saltBytes);

            user.PasswordSalt = Convert.ToBase64String(saltBytes);
            user.PasswordHash = hashedPassword;

            // Remove token after use
            _dbContext.PasswordResetTokens.Remove(resetRecord);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully" });
        }
    }
}
