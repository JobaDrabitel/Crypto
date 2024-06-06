using FP.Core.Database.Models;

namespace FP.Core.Api.ApiDto
{
	public class ReferralResponseDto
	{
		public User User { get; set; }
		public int Inline {  get; set; }
		public decimal DealSum { get; set; }
	}
}
