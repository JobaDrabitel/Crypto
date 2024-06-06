using TronNet.Protocol;

namespace FP.Core.Api.ApiDto;

public class UserCreateDto
{
	public string Password { get; set; } = "";
	public string Login { get; set; } = "";
	public string ReferrerCode { get; set; } = null!;
	public bool IsEmail { get; set; } = true;

	public override string ToString()
	{
		return $"ObjectId - {Login} | Type - UserDto";
	}
}
