namespace FP.Core.Database.Models.ResponseDTO;

public class InvestmentResponseDto
{
    public int Id { get; set; }
    public decimal TotalSum { get; set; }
    public decimal MaxSum { get; set; }
    public decimal TotalYield { get; set; }
    public bool IsEnded { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime LastAccrualDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PacksCount { get; set; }
    public int MaxPacksCount { get; set; }
    public decimal TotalAccrual { get; set; }
    public string Code { get; set; }
    public int UserId { get; set; }
    public int PoolId { get; set; }

    public User User { get; set; }
    public Pool Pool { get; set; }

    public decimal DefiSum { get; set; }
    public decimal DefiPercent { get; set; }

    // Конструктор для создания объекта InvestmentResponseDto из объекта Investment
    public InvestmentResponseDto(Investment investment)
    {
        Id = investment.Id;
        TotalSum = investment.TotalSum;
        MaxSum = investment.MaxSum;
        TotalYield = investment.TotalYield;
        IsEnded = investment.IsEnded;
        StartDate = investment.StartDate;
        LastAccrualDate = investment.LastAccrualDate;
        EndDate = investment.EndDate;
        PacksCount = investment.PacksCount;
        MaxPacksCount = investment.MaxPacksCount;
        TotalAccrual = investment.TotalAccrual;
        Code = investment.Code;
        UserId = investment.UserId;
        PoolId = investment.PoolId;
        User = investment.User;
        Pool = investment.Pool;
    }
}