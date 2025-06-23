using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Models;

public class Livros
{
    public int Id { get; set; }

    [Required]
    public string Titulo { get; set; }

    [Required]
    public string Autor { get; set; }

    [Required]
    public string ISBN { get; set; }

    [Required]
    public string Sinopse { get; set; }
    
    public string? Capa { get; set; }

    // FK to IdentityUser
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    public bool? IsActive { get; set; }
}