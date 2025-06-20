using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevWeb_23774_25961.Models;
using Microsoft.AspNetCore.Identity;
namespace DevWeb_23774_25961.Models;
public class Trocas
{
    public int Id { get; set; }

    // FK for the given book
    public int? IdLivroDado { get; set; }
    [ForeignKey("IdLivroDado")]
    public Livros? LivroDado { get; set; }

    // FK for the received book
    public int? IdLivroRecebido { get; set; }
    [ForeignKey("IdLivroRecebido")]
    public Livros? LivroRecebido { get; set; }

    // User FK strings
    public string? IdVendedor { get; set; }
    public string? IdComprador { get; set; }

    // User navigation properties
    [ForeignKey("IdVendedor")]
    public IdentityUser? Vendedor { get; set; }

    [ForeignKey("IdComprador")]
    public IdentityUser? Comprador { get; set; }

    [Required]
    public EstadoTroca Estado { get; set; } = EstadoTroca.Criada;
    
    public DateTime? Timestamp { get; set; }

    public enum EstadoTroca
    {
        Criada,
        Pendente,
        Aceite,
        Recusada,
    }
}