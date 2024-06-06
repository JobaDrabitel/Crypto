namespace FP.Core.Api.Responses
{
	public class NotFoundResponse : ReturnResponse
	{
		public NotFoundResponse() 
		{
			Message = "Not found";
			Status = false;
		}
	}
}
