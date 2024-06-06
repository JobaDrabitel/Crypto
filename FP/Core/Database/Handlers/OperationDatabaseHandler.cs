using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Models;
using Google.Type;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FP.Core.Database.Handlers;

public class OperationDatabaseHandler
{
	private readonly ILogger<OperationDatabaseHandler> _logger;
	private readonly FpDbContext _dbContext;
	public OperationDatabaseHandler(FpDbContext dbContext, ILogger<OperationDatabaseHandler> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task<ReturnResponse> CreateOperation(int userId, decimal sum, string source, int type, User? user = null)
	{
		ReturnResponse returnResponse;
		try
		{
			Operation operation = new Operation()
			{
				UserId = userId,
				Source = source,
				Partner = user,
				Sum = sum,
				OperationTypeId = type
			};
			_dbContext.Operations.Add(operation);
			await _dbContext.SaveChangesAsync();
			returnResponse = new OkResponse<Operation>(operation);
		}
		catch (Exception ex) 
		{
			returnResponse = new InternalErrorResponse();
		}
		return returnResponse;
	}
	public async Task<ReturnResponse> GetOperationsByUserId(int userId, HistoryFiltersDto operationDto)
	{
		ReturnResponse returnResponse;
		var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		var startDate = DateOnly.FromDateTime(dateTime.AddSeconds(operationDto.Start));
		var endDateTime = DateOnly.FromDateTime(dateTime.AddSeconds(operationDto.End));
		try
		{
			var operations =_dbContext.Operations
				.Include(o=> o.Partner)
				.Include(o=>o.OperationType)
				.Where(o => o.UserId == userId && o.Date > startDate && o.Date < endDateTime).ToList();

			if (operationDto.OperationId != null)
				operations = operations.Where(o => o.OperationTypeId == operationDto.OperationId).ToList();

			returnResponse = new OkResponse<List<Operation>>(operations);
		}
		catch (Exception ex) 
		{ 
			returnResponse = new InternalErrorResponse(); 
		}
		return returnResponse;
	}
	public async Task<ReturnResponse> GetDealSumFromReferral(int referrerId, int referralId)
	{
		ReturnResponse returnResponse;
		try
		{
			var operations = _dbContext.Operations
				.Where(o => o.UserId == referrerId && o.PartnerId==referralId && o.OperationTypeId == (int)OperationTypeEnum.RefBonus)
				.ToList();
			if (operations != null)
				returnResponse = new OkResponse<decimal>(operations.Sum(o=>o.Sum));
			else
				returnResponse = new OkResponse<decimal>(0);
		}
		catch (Exception ex)
		{
			returnResponse = new InternalErrorResponse();
		}
		return returnResponse;
	}
}
