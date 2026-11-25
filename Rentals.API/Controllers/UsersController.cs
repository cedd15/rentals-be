using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentals.API.Models;
using Rentals.API.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUser()
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

    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserWriteRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.UserName = request.UserName;
        user.Password = request.Password;

        try
        {
            await _context.SaveChangesAsync();
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

        return NoContent();
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(UserWriteRequest request)
    {
        var user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateException dbUpdateEx) when (dbUpdateEx.InnerException is SqlException sqlEx &&
                                  (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            if (dbUpdateEx.InnerException is SqlException)
                return BadRequest("A user with the same username already exists.");
        }

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int? id)
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

    private bool UserExists(int? id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
