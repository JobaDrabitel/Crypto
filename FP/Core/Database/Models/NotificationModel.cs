using FP.Core.Api.Services;

namespace FP.Core.Database.Models;

public interface INotify
{
    int UserId { get; set; }
    public string NotifyType { get; set; }
    string ToString();
}
public class TopUpNotify : INotify
{
    public int UserId { get; set; }
    public decimal Balance { get; set; }
    public string NotifyType { get; set; } = NotificationType.TopUp.ToString();

    public TopUpNotify(int userId, decimal balance)
    {
        UserId = userId;
        Balance = balance;
    }

    public override string ToString()
    {
        return $"Top up has been completed successfully. Current balance - {Balance}";
    }
}

public class TransferNotify : INotify
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Recipient { get; set; }
    public decimal Balance { get; set; }
    public string NotifyType { get; set; } = NotificationType.Transfer.ToString();

    public TransferNotify(int userId, decimal amount, string recipient, decimal balance)
    {
        UserId = userId;
        Amount = amount;
        Recipient = recipient;
        Balance = balance;
    }

    public override string ToString()
    {
        return $"Transfer has been completed successfully. Amount: {Amount}, Recipient: {Recipient}, Current balance: {Balance}";
    }
}
public class WithdrawNotify : INotify
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
	public string NotifyType { get; set; } = NotificationType.Withdraw.ToString();

	public WithdrawNotify(int userId, decimal amount, decimal balance)
    {
        UserId = userId;
        Amount = amount;
        Balance = balance;
    }

    public override string ToString()
    {
        return $"Withdrawal successful. Amount: {Amount}, Current balance: {Balance}";
    }
}

public class ReferralNotify : INotify
{
    public int UserId { get; set; }
    public string Nickname { get; set; }
    public int Generation { get; set; }
	public string NotifyType { get; set; } = NotificationType.Referral.ToString();
	public ReferralNotify(int userId, string nickname, int generation)
    {
        UserId = userId;
        Nickname = nickname;
        Generation = generation;
    }

    public override string ToString()
    {
        return $"New user in your structure: Nickname {Nickname}, Generation {Generation}";
    }
}

public class DividendNotify : INotify
{
    public int UserId { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
	public string NotifyType { get; set; } = NotificationType.Dividend.ToString();
    public decimal Balance { get; set; }
	public DividendNotify(int userId, string type, string source, decimal balance)
    {
        UserId = userId;
        Type = type;
        Source = source;
    }

    public override string ToString()
    {
        return $"Dividend received. Type: {Type}, Source: {Source}. Balance:{Balance}";
    }
}
