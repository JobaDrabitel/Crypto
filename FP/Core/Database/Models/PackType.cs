using Newtonsoft.Json;

namespace FP.Core.Database.Models;

public class PackType
{
	public int Id { get; set; }
	public decimal Yield { get; set; }
	public string Name { get; set; } = string.Empty;
	public int MaxDuration { get; set; }
	public int MinDuration { get; set; }
	public decimal TotalIncome { get; set; }
	public decimal ActiveSum { get; set; }
	public string? Avatar { get; set; }

	[JsonIgnore] public ICollection<Pack> Packs = new List<Pack>();
}
