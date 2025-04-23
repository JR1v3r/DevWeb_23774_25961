using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevWeb_23774_25961.Models;

public class Trades
{
    public int Id { get; set; }

    [Required]
    public int UsersBooks_Id { get; set; }
    [ForeignKey("UsersBooks_Id")]
    public UsersBooks InventorySlot { get; set; }
    
    public string PreviousOwner { get; set; }
    public string CurrentOwner { get; set; }

    [Required]
    public TradeStatus Status { get; set; } = TradeStatus.Pending;
}