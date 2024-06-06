using System.ComponentModel.DataAnnotations.Schema;

namespace FP.Core.Database.Models;

public class Notify
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public string Type { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    [ForeignKey("UserId")] public User User { get; set; } = null!;
}