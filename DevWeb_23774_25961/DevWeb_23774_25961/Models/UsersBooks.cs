using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Models;

public class UsersBooks
{
    public int Id { get; set; }
    
    [Required]
    public string Users_Id { get; set; }
    [ForeignKey("Users_Id")]
    public IdentityUser User { get; set; }

    [Required]
    public int Books_Id { get; set; }
    [ForeignKey("Books_Id")]
    public Books Book { get; set; }
}
