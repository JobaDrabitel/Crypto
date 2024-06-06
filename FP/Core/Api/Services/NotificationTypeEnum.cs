using System.Runtime.Serialization;

namespace FP.Core.Api.Services;

public enum NotificationType
{
	[EnumMember(Value = "TopUp")]
	TopUp,
	[EnumMember(Value = "Transfer")]
	Transfer,
	[EnumMember(Value = "Withdraw")]
	Withdraw,
	[EnumMember(Value = "Referral")]
	Referral,
	[EnumMember(Value = "Dividend")]
	Dividend,

}
