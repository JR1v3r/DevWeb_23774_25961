using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Models;

public class Books
{
    public int Id { get; set; }

    [Required]
    public string Titulo { get; set; }

    [Required]
    public string Autor { get; set; }

    [Required]
    public string Descrição { get; set; }

    [Required]
    public int ISBN { get; set; }

    [Required]
    public string CapaPath { get; set; }
    
    public string? CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public IdentityUser Creator { get; set; }
    
    public DateTime? CreatedOn { get; set; }
    
    public string? UpdatedBy { get; set; }
    [ForeignKey("UpdatedBy")]
    public IdentityUser? Updater { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    
 
    public bool? IsActive { get; set; }
}
