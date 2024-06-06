namespace FP.Core.Api.ApiDto;

public class HistoryFiltersDto
{
    public long Start { get; set; }
    public long End { get; set; }
    public int? OperationId { get; set; }
    public int? PoolId { get; set; }
}