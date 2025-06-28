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

    // PUBLIC: No auth needed
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Livros>>> GetAllLivros()
    {
        var livros = await _context.Livros.ToListAsync();
        return Ok(livros);
    }

    // PUBLIC: No auth needed
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Livros>> GetLivro(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();

        return Ok(livro);
    }

    // PROTECTED endpoints, only logged-in users:
    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Livros>>> GetMyLivros()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var meusLivros = await _context.Livros.Where(l => l.UserId == user.Id).ToListAsync();
        return Ok(meusLivros);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Livros>> CreateLivro([FromBody] Livros livro)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        livro.UserId = user.Id;
        livro.IsActive = true;

        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, livro);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> EditLivro(int id, [FromBody] Livros livro)
    {
        if (id != livro.Id) return BadRequest();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

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
            else throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLivro(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        if (livro.UserId != user.Id) return Forbid();

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LivrosExists(int id)
    {
        return _context.Livros.Any(e => e.Id == id);
    }

    // NEW! Login endpoint that returns JWT token
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid username or password.");

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}