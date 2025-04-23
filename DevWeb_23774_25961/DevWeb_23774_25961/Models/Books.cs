using System.ComponentModel.DataAnnotations;

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

}

