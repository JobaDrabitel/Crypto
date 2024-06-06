using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace FP.Core.Api.Controllers
{
	[ApiController]
	[Route(("api/notify"))]
	public class NotifyController : Controller
	{
		private readonly JwtService _jwtService;
		private readonly NotificationDatabaseHandler _notificationDatabaseHandler;

		public NotifyController(JwtService jwtService, NotificationDatabaseHandler notificationDatabaseHandler)
		{
			_notificationDatabaseHandler = notificationDatabaseHandler;
			_jwtService = jwtService;
		}


		[HttpGet("read/{id}")]
		public async Task<IActionResult> ReadNotify(int id)
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
			var result = await _notificationDatabaseHandler.ReadNotifyAsync(id);
			if (result == null)
				BadRequest(new InternalErrorResponse());
			return Ok(new OkResponse<Notify>(result));
		}
		[HttpGet("readAll")]
		public async Task<IActionResult> ReadAllNotifies()
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

			var notifies = await _notificationDatabaseHandler.ReadAllNotifiesByUser(userId);
			if (notifies == null)
				return BadRequest(new InternalErrorResponse());

			return Ok(new OkResponse<List<Notify>>(notifies));
		}
		
		[HttpGet("get/{id}")]
		public async Task<IActionResult> GetNotify(int id)
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


			var notify = await _notificationDatabaseHandler.GetNotifyAsync(id);
			if (notify == null)
				return NotFound(new InternalErrorResponse());

			return Ok(new OkResponse<Notify>(notify));
		}

		[HttpGet("getAll")]
		public async Task<IActionResult> GetAllNotify()
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
			
			var notify = await _notificationDatabaseHandler.GetAllNotifyAsync(userId);
			return notify.Status ? Ok(notify) : BadRequest(notify);
		}
	}
}
