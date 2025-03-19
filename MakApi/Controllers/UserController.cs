using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MakApi.Data;
using MakApi.Data.Models;
using MakApi.Services;

namespace MakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public UserController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        // 检查 Email 是否已存在
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        
        try
        {
            await _context.SaveChangesAsync();
            // Send email notification
            await _emailService.SendUserCreatedEmailAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return BadRequest(new { message = "Email already exists" });
            }
            throw;
        }
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // 检查新的 Email 是否与其他用户重复
        if (existingUser.Email != user.Email)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != id);
            if (emailExists)
            {
                return BadRequest(new { message = "Email already exists" });
            }
        }

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            // Send email notification
            await _emailService.SendUserUpdatedEmailAsync(existingUser);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
} 