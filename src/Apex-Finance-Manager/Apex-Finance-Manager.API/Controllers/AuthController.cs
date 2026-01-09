using Apex_Finance_Manager.Data.DBContext;
using Apex_Finance_Manager.Entities.DTOs;
using Apex_Finance_Manager.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apex_Finance_Manager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
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
}
