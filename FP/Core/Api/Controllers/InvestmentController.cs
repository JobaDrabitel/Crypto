using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using FP.Core.Database.Models.ResponseDTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FP.Core.Api.Controllers;

[ApiController]
[Route("api/investment")]
public class InvestmentController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly ICryptoApiProvider _cryptoProvider;
    private readonly ILogger<InvestmentController> _logger;
    private readonly UserDatabaseHandler _userDatabaseHandler;
    private readonly InvestmentDatabaseHandler _investmentDatabaseHandler;
    private readonly PackDatabaseHandler _packDatabaseHandler;

    public InvestmentController(JwtService jwtService, ILogger<InvestmentController> logger, UserDatabaseHandler userDatabaseHandler,
        ICryptoApiProvider cryptoApiProvider, InvestmentDatabaseHandler investmentDatabaseHandler, PackDatabaseHandler packDatabaseHandler)
    {
        _cryptoProvider = cryptoApiProvider;
        _userDatabaseHandler = userDatabaseHandler;
        _investmentDatabaseHandler = investmentDatabaseHandler;
        _packDatabaseHandler = packDatabaseHandler;
        _jwtService = jwtService;
        _logger = logger;
    }
    
    [HttpGet("generateCode/sum={sum}")]
    public IActionResult GenerateCode(decimal sum)
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

        if (sum <= 0)
            return BadRequest(new InvalidData("sum"));

        var code = CodeService.GenerateCode(PromocodeType.Common, sum);
        
        return Ok(new OkResponse<string>(code));
    }

    [HttpPut("transferFound/investmentId={id:int}&amount={amount:decimal}")]
    public async Task<IActionResult> TransferToFound(int id, decimal amount)
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

        decimal commission = 0.01m;
        var response = await _investmentDatabaseHandler.TransferBalanceToFound(id, amount-commission);
        return response.Status ? Ok(response) : BadRequest(response);
    }
    
    [HttpPut("transferIncome/investmentId={id:int}")]
    public async Task<IActionResult> TransferToIncome(int id, decimal amount)
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

        var response = await _investmentDatabaseHandler.TransferBalanceToIncome(userId, id);
        return response.Status ? Ok(response) : BadRequest(response);
    }
    
    [HttpPost($"create/code={{code}}&poolId={{poolId}}")]
    public async Task<IActionResult> CreateInvestmentAsync([FromBody] PackDto packDtos, string code, int poolId)
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
        if(packDtos.PackDtos is null || packDtos.PackDtos.Length < 5 && packDtos.InvestmentCode.Length == 42)
            return BadRequest(new InvalidData("Packs"));

        var investment = await _investmentDatabaseHandler.CreateInvestment(userId, code, poolId);
        
        if (!investment.Status)
            return BadRequest(investment);

        if(packDtos.InvestmentCode.Length == 42)
            await _packDatabaseHandler.CreatePacks(packDtos, userId);

        return investment.Status ? Ok(investment) : BadRequest(investment);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllInvestmentAsync()
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

        var response = await _investmentDatabaseHandler.GetAllInvestments(userId);
        if (!response.Status)
            return BadRequest(response);
        
        var listOfInvestments = response as OkResponse<List<Investment>>;

        var listOfResponses = new List<InvestmentResponseDto>();
        foreach (var item in listOfInvestments?.ObjectData)
        {
            var newResp = new InvestmentResponseDto(item);
            var defiTrans = _investmentDatabaseHandler.GetAllDefiTransactions(item.Id);
            if (newResp == null) continue;
            newResp.DefiPercent = defiTrans.Sum(d => d.AdditionPercent);
            newResp.DefiSum = defiTrans.Sum(d => d.Sum);
            listOfResponses.Add(newResp);
        }
        return Ok(new OkResponse<List<InvestmentResponseDto>>(listOfResponses));
    }
    
    [HttpGet("defiAll/{investCode}")]
	public async Task<IActionResult> GetAllDefiAsync(string investCode)
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

		var result = await _investmentDatabaseHandler.GetAllInvestments(userId);
        if(!result.Status)
            return BadRequest(result);

        var invest = await _investmentDatabaseHandler.GetInvestmentByCode(investCode);
        if (invest == null)
            return BadRequest(new NotFoundResponse());

        var defiTransactions = _investmentDatabaseHandler.GetAllDefiTransactions(invest.Id);
        
        var response = new OkResponse<List<DefiTransactions>>(defiTransactions);
        
		return response.Status ? Ok(response) : BadRequest(response);
	}
    
    [HttpPut("closeDefi/{id}")]
	public async Task<IActionResult> CloseDefiAsync(int id)
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

		var response = await _investmentDatabaseHandler.TransferFromFound(id);
		return response.Status ? Ok(response) : BadRequest(response);
	}

    [HttpGet("pools")]
    public IActionResult GetAllPools()
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

        var response = _investmentDatabaseHandler.GetPools();
        return response.Status ? Ok(response) : BadRequest(response);
    }
    
    [HttpPut("close/{id:int}")]
    public async Task<IActionResult> CloseInvestAsync(int id)
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

        var userReponse = await _userDatabaseHandler.GetUserById(userId);
        if (!userReponse.Status)
            return BadRequest(userReponse);
        var response = await _investmentDatabaseHandler.CloseInvest(id, ((OkResponse<User>)userReponse).ObjectData);
        return response.Status ? Ok(response) : BadRequest(response);
    }
}