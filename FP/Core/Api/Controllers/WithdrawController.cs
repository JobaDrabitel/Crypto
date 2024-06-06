using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using static TronNet.Protocol.Transaction.Types;

namespace FP.Core.Api.Controllers
{
    [ApiController]
	[Route("api/withdraw")]
	public class WithdrawController : Controller
	{

		private readonly ILogger<PackController> _logger;
		private readonly JwtService _jwtService;
		private readonly WithdrawDatabaseHandler _databaseHandler;
		private readonly UserDatabaseHandler _userDatabaseHandler;

		public WithdrawController(WithdrawDatabaseHandler databaseHandler, ILogger<PackController> logger, JwtService jwtService, UserDatabaseHandler userDatabaseHandler)
		{

			_databaseHandler = databaseHandler;
			_logger = logger;
			_jwtService = jwtService;
			_userDatabaseHandler = userDatabaseHandler;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateWithdrawAsync([FromBody] WithdrawDto withdrawDto)
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			if (await _userDatabaseHandler.GetUserById(userId) is not OkResponse<User> response || !response.Status)
				return BadRequest(new InvalidData("userId"));
			if(withdrawDto.Sum < 1)
				return BadRequest(new InvalidData("withdrawSum"));
			var resp = await _userDatabaseHandler.WithdrawCrypto(userId, withdrawDto.Sum);
			if (!resp.Status)
				return BadRequest(resp);
			var result = await _databaseHandler.CreateWithdraw(withdrawDto, userId);
			if (result == null)
				return BadRequest(new InvalidData("WithdrawDto"));
			return resp.Status ? Ok(new OkResponse<Withdraw>(result)) : BadRequest(resp);

		}

		[HttpGet("userWaiting")]
		public async Task<IActionResult> GetWaiting()
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));

			var result = await _databaseHandler.GetUsersWaiting(userId);

			return result.Status ? Ok(result) : BadRequest(result);
		}
		[HttpPut("realize/{id}")]
		public async Task<IActionResult> RealizeWithdrawAsync(int id)
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			if (await _userDatabaseHandler.GetUserById(userId) is not OkResponse<User> response || !response.Status)
				return BadRequest(new InvalidData("userId"));
			if (!response.ObjectData.IsAdmin)
				return BadRequest(new InvalidData("forbidden"));

			var result = await _databaseHandler.RealizeWithdraw(id);
			return result.Status ? Ok(result) : BadRequest(result);
		}

		[HttpPut("reject/id")]
		public async Task<IActionResult> RejectAsync(int id)
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			if (await _userDatabaseHandler.GetUserById(userId) is not OkResponse<User> response || !response.Status)
				return BadRequest(new InvalidData("userId"));
			if (!response.ObjectData.IsAdmin)
				return Forbid();

			var result = await _databaseHandler.RejectById(id);
			if (!result.Status)
				return BadRequest(result);

			var user = result as OkResponse<Withdraw>;

			result = await _userDatabaseHandler.UserTopUpInternal(userId, user.ObjectData.Sum);
			return result.Status ? Ok(result) : BadRequest(result);
		}
		
		[HttpGet("all")]
		public async Task<IActionResult> GetAllWithdrawsAsync()
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			if (await _userDatabaseHandler.GetUserById(userId) is not OkResponse<User> response || !response.Status)
				return BadRequest(new InvalidData("userId"));
			if (!response.ObjectData.IsAdmin)
				return Forbid();
			
			var result = await _databaseHandler.GetAllWithdraws();
			return result != null ? Ok(result) : BadRequest(result);
		}
		[HttpGet("getUnrealized")]
		public async Task<IActionResult> GetUnrealizedWithdrawsAsync()
		{
			var jwt = Request.Cookies["jwt"];
			if (jwt == null)
				return Unauthorized();
			var token = _jwtService.Verify(jwt);
			var isSuccess = int.TryParse(token.Issuer, out var userId);
			if (!isSuccess)
				return BadRequest(new InvalidData("Token"));
			if (await _userDatabaseHandler.GetUserById(userId) is not OkResponse<User> response || !response.Status)
				return BadRequest(new InvalidData("userId"));
			if (!response.ObjectData.IsAdmin)
				return Forbid();
			var result = await _databaseHandler.GetUnrealizedWithdraws();
			return result != null ? Ok(result) : BadRequest(result);
		}
	}
	
}
