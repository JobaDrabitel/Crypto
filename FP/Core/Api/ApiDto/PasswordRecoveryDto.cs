namespace FP.Core.Api.ApiDto
{
	public class PasswordRecoveryDto
	{
		public string Code { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
	}
}
