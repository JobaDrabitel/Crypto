namespace FP.Core.Api.ApiDto
{
	public class CreateTransactionDto
	{
		public bool FromAgent { get; set; }
		public bool ToAgent { get; set; } = false;
		public string Code { get; set; } = string.Empty;
		public decimal Sum { get; set; }
		public string Subject { get; set; } = "";
		public string TextBeforeCode { get; set; } = "";
		public string TextAfterCode { get; set; } = "";
	}
}
