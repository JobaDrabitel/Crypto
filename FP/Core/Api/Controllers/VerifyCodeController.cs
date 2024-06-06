using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FP.Core.Api.Controllers;

[ApiController]
[Route("api/verify")]
public class VerifyCodeController : ControllerBase
{

	private readonly JwtService _jwtService;
	private readonly ILogger<InvestmentController> _logger;
	private readonly VerificationCodeDatabaseHandler _verificationCodeDatabaseHandler;
	private readonly UserDatabaseHandler _userDatabaseHandler;

	public VerifyCodeController(JwtService jwtService, ILogger<InvestmentController> logger,
		VerificationCodeDatabaseHandler verificationCodeDatabaseHandler, UserDatabaseHandler userDatabaseHandler)
	{
		_userDatabaseHandler = userDatabaseHandler;
		_verificationCodeDatabaseHandler = verificationCodeDatabaseHandler;
		_jwtService = jwtService;
		_logger = logger;
	}


	[HttpGet("get/{code}")]
	public async Task<IActionResult> GetUnusedCodes(string code)
	{
		var result = await _verificationCodeDatabaseHandler.FindCode(code);
		if (!result.Status)
			BadRequest(result);
		return Ok((result as OkResponse<VerificationCode>).ObjectData.UserId);
	}

	[HttpPost("create")]
	public async Task<IActionResult> CreateVerificationCode()
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
		
		var random = new Random();
		var code = random.Next(100000, 999999);
		var result = await _verificationCodeDatabaseHandler.Create(code.ToString(), userId);
		if (!result.Status)
			BadRequest(result);
		return Ok((result as OkResponse<VerificationCode>));
	}

}

