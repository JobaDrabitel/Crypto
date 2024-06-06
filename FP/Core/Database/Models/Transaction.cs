using MimeKit.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FP.Core.Database.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal DealSum { get; set; }
    public bool FromAgent { get; set; }
    public bool ToAgent { get; set; } = false;
    public int FromUserId { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public int ToUserId { get; set; }
    [ForeignKey("FromUserId")]
    public User FromUser { get; set; } = null!;
    [ForeignKey("ToUserId")]
    public User ToUser { get; set; } = null!;

}