using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevWeb_23774_25961.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Livros> Livros { get; set; }
    public DbSet<Trocas> Trocas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Trocas>()
            .HasOne(t => t.Vendedor)
            .WithMany()
            .HasForeignKey(t => t.IdVendedor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trocas>()
            .HasOne(t => t.Comprador)
            .WithMany()
            .HasForeignKey(t => t.IdComprador)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trocas>()
            .HasOne(t => t.LivroDado)
            .WithMany()
            .HasForeignKey(t => t.IdLivroDado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Trocas>()
            .HasOne(t => t.LivroRecebido)
            .WithMany()
            .HasForeignKey(t => t.IdLivroRecebido)
            .OnDelete(DeleteBehavior.Restrict);
    }


    
    
}

