using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookTradesProject.Models;

public class Livros
{
    public int Id { get; set; }
    
    [Required]
    public string Titulo { get; set; }
    
    [Required]
    public string Autor { get; set; }
    
    [Required]
    public int ISBN { get; set; }
    
    [Required]
    public string Sinopse { get; set; }
    
    [Required]
    public string Capa { get; set; }
    
    //FK para o dono atual do livro
    public string? DonoId { get; set; }
    
    [ValidateNever]
    public IdentityUser? DonoId_FK { get; set; }
    
    //Mecanismo de Soft-Delete (impede que os logs das trocas sejam comprometidos)
    public bool? IsActive { get; set; }
}