using FP.Core.Api.Services;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FP.Core.Database.Models;

public class Promocode
{
    public int Id { get; set; } 
    public string Code { get; set; } = "";
    public DateTime CreationTime {  get; set; } = DateTime.UtcNow;
    public bool IsActived { get; set; } = false; 

    public string Type { get; set; } = string.Empty;
    public int UserId { get; set; }
    public decimal DealSum { get; set; } = 0.0m;
	[ForeignKey("UserId")]
	[JsonIgnore] public User User { get; set; }
}