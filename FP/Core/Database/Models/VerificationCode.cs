using System.ComponentModel.DataAnnotations.Schema;

namespace FP.Core.Database.Models;

public class VerificationCode
{
    public int Id { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; } = true;
    public int? UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}