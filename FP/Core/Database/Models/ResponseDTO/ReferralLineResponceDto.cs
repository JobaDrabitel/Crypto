namespace FP.Core.Database.Models.ResponseDTO;

public class ReferralLineResponseDto : ReferralTreeResponseDto
{
    public User User { get; set; } = null!;
}