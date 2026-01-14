using Apex_Finance_Manager.Data.DBContext;
using Apex_Finance_Manager.Entities.DTOs;
using Apex_Finance_Manager.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Apex_Finance_Manager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly IConfiguration _config;
    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        // 1. Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            return BadRequest("A user with this email already exists.");
        }

        // 2. Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // 3. Map DTO to Entity
        var user = new User
        {
            Email = model.Email,
            PasswordHash = passwordHash,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Save to Database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful!" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        // 1. Find user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null) return Unauthorized("Invalid email or password.");

        // 2. Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        // 3. Create Claims
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.GivenName, user.FirstName)
    };

        // 4. Generate Token
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            firstName = user.FirstName,
            lastName=user.LastName
        });
    }
}
