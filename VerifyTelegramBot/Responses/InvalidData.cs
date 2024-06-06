namespace FP.Core.Api.Responses
{
	public class InvalidData : ReturnResponse
	{
		public InvalidData(string data) 
		{
			Message = $"Invalid data: {data}";
			Status = false;
		}
	}
}
