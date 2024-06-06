using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FP.Core.Api.Controllers
{
    [ApiController]
	[Route("api/wallet")]
	public class WalletController : ControllerBase
	{
		private readonly JwtService _jwtService;
		private readonly ILogger<WalletController> _logger;
		private readonly UserDatabaseHandler _userDatabaseHandler;

		public WalletController(JwtService jwtService, ILogger<WalletController> logger, 
			UserDatabaseHandler userDatabaseHandler)
		{
			_userDatabaseHandler = userDatabaseHandler;
			_jwtService = jwtService;
			_logger = logger;
		}

		[HttpGet("balance")]
		public async Task<IActionResult> GetWalletBalanceAsync()
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			if (token == null)
				return Unauthorized();
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			
			var balance = await _userDatabaseHandler.CheckWalletsBalance(userId);
			
			return balance == null ? 
				BadRequest(new InvalidData("balance")) : 
				Ok(new OkResponse<decimal>(balance.Value));
		}
	}
}
