using System.ComponentModel.DataAnnotations.Schema;

namespace FP.Core.Database.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public int OperationTypeId { get; set; }
		[ForeignKey("OperationTypeId")]
		public OperationType OperationType { get; set; }
        public decimal Sum { get; set; }
        public bool IsAgentBalance { get; set; } = false;
        public string Source { get; set; } = string.Empty;
        public int? PartnerId { get; set; }
		[ForeignKey("PartnerId")]
		public User? Partner { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
