using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DevWeb_23774_25961.Models;

public class Trades
{
    public int Id { get; set; }

    [Required]
    public int UsersBooks_Id { get; set; }
    [ForeignKey("UsersBooks_Id")]
    public UsersBooks InventorySlot { get; set; }

    [Required]
    public TradeStatus Status { get; set; } = TradeStatus.Pending;
    
    [Required]
    public string CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public IdentityUser Creator { get; set; }
    
    [Required]
    public DateTime CreatedOn { get; set; }
    
 
    public string? UpdatedBy { get; set; }
    [ForeignKey("UpdatedBy")]
    public IdentityUser? Updater { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
}