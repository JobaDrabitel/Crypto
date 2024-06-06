namespace FP.Core.Api.Responses;

public class NotVerified : ReturnResponse
{
    public NotVerified()
    {
        Message = "Not verified";
        Status = false;
    }
}