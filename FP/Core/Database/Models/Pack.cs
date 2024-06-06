using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FP.Core.Database.Models
{
	public class Pack
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
		public DateTime EndDate {  get; set; }
		public bool HasLastAccrual { get; set; } = false;
		public decimal Yield { get; set; } = 0.0m;
		public decimal CurrentYield { get; set; } = 0.0m;
		public bool ByMaxDuration { get; set; }
		public int InvestmentId { get; set; }
		[ForeignKey("InvestmentId")]
		public Investment Investment { get; set; } = null!;
		
		public int PackTypeId { get; set; }
		[ForeignKey("PackTypeId")]
		public PackType PackType { get; set; } = null!;
	}
}
