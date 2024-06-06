using FP.Core.Api.ApiDto;
using FP.Core.Api.Controllers;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Responses;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;


namespace FP.Core.Api.Services;

public class TestDataService
{
	private readonly UserDatabaseHandler _userDatabaseHandler;
	private readonly InvestmentDatabaseHandler _investmentDatabaseHandler;
	private readonly PackDatabaseHandler _packDatabaseHandler;

	public TestDataService(UserDatabaseHandler userDatabaseHandler, InvestmentDatabaseHandler investmentDatabaseHandler, PackDatabaseHandler packDatabaseHandler)
	{
		_userDatabaseHandler = userDatabaseHandler;
		_investmentDatabaseHandler = investmentDatabaseHandler;
		_packDatabaseHandler = packDatabaseHandler;
	}
	/*public async Task<User?> CreateTestUser(TestUserCreateDto userCreateDto)
	{
		try
		{
			var stringBuilder = new RandomStringBuilder();
			var user = (await _userDatabaseHandler.CreateUser(new UserCreateDto
			{
				Login = stringBuilder.Create(10),
				IsEmail = true,
				Password = stringBuilder.Create(10),
				ReferrerCode = userCreateDto.ReferrerCode
			}) as OkResponse<User>)?.ObjectData;
			
			await _userDatabaseHandler.UpdateUser(user.Id, new UserUpdateDto());
			await _userDatabaseHandler.ConfirmEmailAsync(user.Id);
			await _userDatabaseHandler.UserTopUpInternal(user.Id, 200);
			var code = CodeService.GenerateCode(PromocodeType.Common);
			var investment = await CreateTestInvestment(user.Id, code, userCreateDto);

			return user;
		}
		catch (Exception ex)
		{
			return null;
		}
	}*/

	/*private async Task<Investment?> CreateTestInvestment(int userId, string code, TestUserCreateDto userCreateDto)
	{
		var investment = await _investmentDatabaseHandler.CreateInvestment(userId, code, 2);
		var packDto = new PackDto
		{
			InvestmentCode = code,
			PackTypeId = (await _packDatabaseHandler.GetAllPackTypes()).FirstOrDefault(p => p.Name == "Test")!.Id,
			DealSum = userCreateDto.Sum
		};
		var response = await _packDatabaseHandler.CreatePacks(new[] { packDto }, userId);

		return response is not OkResponse<Pack> pack ? null : (investment as OkResponse<Investment?>)?.ObjectData;
	}*/
}
