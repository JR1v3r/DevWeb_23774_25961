using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevWeb_23774_25961.Data;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DevWeb_23774_25961.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LivrosApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public LivrosApiController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Livros>>> GetAllLivros()
    {
        return await _context.Livros.ToListAsync();
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Livros>> GetLivro(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        return livro == null ? NotFound() : Ok(livro);
    }

    [HttpGet("user")]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    public async Task<ActionResult<IEnumerable<Livros>>> GetMyLivros()
    {
        // CORRECT: Get the ID (GUID) from NameIdentifier claim
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null) return Unauthorized("User not found");

        return await _context.Livros.Where(l => l.UserId == user.Id).ToListAsync();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    public async Task<ActionResult<Livros>> CreateLivro([FromBody] Livros livro)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized("Invalid credentials");

        livro.UserId = user.Id;
        livro.IsActive = true;

        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, livro);
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    public async Task<IActionResult> EditLivro(int id, [FromBody] Livros livro)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != livro.Id) return BadRequest();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized("Invalid credentials");

        var livroExist = await _context.Livros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        if (livroExist == null) return NotFound();
        if (livroExist.UserId != user.Id) return Forbid();

        livro.UserId = user.Id;
        _context.Entry(livro).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LivrosExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    public async Task<IActionResult> DeleteLivro(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized("Invalid credentials");

        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        if (livro.UserId != user.Id) return Forbid();

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);
        return Ok(new { 
            token,
            expiresIn = DateTime.UtcNow.AddMinutes(15) // Shorter expiry
        });
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // 15-minute expiry
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool LivrosExists(int id) => _context.Livros.Any(e => e.Id == id);
}

public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}