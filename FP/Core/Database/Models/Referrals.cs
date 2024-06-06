using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FP.Core.Database.Models;

public class Referral
{
    public int Id { get; set; }
    
    public int ReferrerId { get; set; }
    [ForeignKey("ReferrerId")]
    [JsonIgnore] public User Referrer { get; set; } = null!;
    
    public int Inline { get; set; }
    
    public int RefId { get; set; }
    [ForeignKey("RefId")]
    [JsonIgnore] public User Ref { get; set; } = null!;
}