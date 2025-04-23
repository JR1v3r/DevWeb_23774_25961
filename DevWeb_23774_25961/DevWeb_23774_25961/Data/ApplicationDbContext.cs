using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevWeb_23774_25961.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    public DbSet<Books> Books { get; set; }
    public DbSet<UsersBooks> UsersBooks { get; set; }
    public DbSet<Trades> Trades { get; set; }

}