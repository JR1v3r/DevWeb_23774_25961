using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookTradesProject.Models;

public class Trocas
{
    public int Id { get; set; }
    
    //FK para o livro dado na troca
    [Required]
    public string LivroDado { get; set; }
    [ValidateNever]
    public Livros? LivroDado_FK { get; set; }
    
    //FK para o livro recebido em troca
    public string? LivroRecebido { get; set; }
    [ValidateNever]
    public Livros? LivroRecebido_FK { get; set; }
    
    //FK para o "vendedor"
    public string? Vendedor { get; set; }
    [ValidateNever]
    public IdentityUser? Vendedor_FK { get; set; }
    
    //FK para o "comprador"
    public string? Comprador { get; set; }
    [ValidateNever]
    public IdentityUser? Comprador_FK { get; set; }
    
    //estado da troca
    [Required]
    public EstadoTroca Estado { get; set; } = EstadoTroca.Pendente;
    
    //timestamp de quando ocorreu a troca
    [Required]
    public DateTime Timestamp { get; set; }
    
    
    
    //Enumerador para os estados da troca
    public enum EstadoTroca
    {
        Pendente,
        Aceite,
        Recusada,
    }
}