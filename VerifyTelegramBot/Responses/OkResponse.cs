namespace FP.Core.Api.Responses
{
	public class OkResponse<T> : ReturnResponse
	{
		public T? ObjectData { get; set; }
		public OkResponse(T? data) 
		{
			ObjectData = data;
		}
	}
}
