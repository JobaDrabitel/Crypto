namespace FP.Core.Api.Responses
{
	public class InternalErrorResponse : ReturnResponse
	{
		public InternalErrorResponse() 
		{
			Message = "Internal Error";
			Status = false;
		}
	}
}
