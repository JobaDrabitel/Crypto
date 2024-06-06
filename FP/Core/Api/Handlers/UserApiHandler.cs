using Loger;

namespace FP.Core.Api.Handlers;

public class UserApiHandler
{
	private LogService<UserApiHandler> _loger;

	public UserApiHandler() => _loger = new();

	//public async Task<string> CreateUser(UserDto userData)
	//{
	//    _loger.LogAction($"Start to create new user. {userData}", new string[]
	//    {
	//        "Registration...",
	//        $"{userData}"
	//    });

	//    var result = await UserDatabaseHandler.CreateUser(userData);

	//    if (result == "Ok")
	//    {
	//        _loger.LogAction($"UserDatabaseHandler created new user", new string[] 
	//        { 
	//            $"{userData}" 
	//        });
	//    }
	//    else
	//    {
	//        _loger.LogAction($"UserDatabaseHandler cannot create user", new string[]
	//        {
	//            $"{userData}"
	//        });
	//    }

	//    return result;
	//}

	//internal async Task<string> LoginUser(UserDto userData)
	//{
	//    _loger.LogAction($"Start to login user", new string[]
	//    {
	//        "Login...",
	//        $"{userData}"
	//    });

	//    var result = await UserDatabaseHandler.LoginUser(userData);

	//    if (result == "Ok")
	//    {
	//        _loger.LogAction($"UserDatabaseHandler login user", new string[] 
	//        { 
	//            $"{userData}" 
	//        });
	//    }
	//    else
	//    {
	//        _loger.LogAction($"UserDatabaseHandler cannot login user", new string[]
	//        {
	//            $"{userData}"
	//        });
	//    }

	//    return result;
	//}
}
