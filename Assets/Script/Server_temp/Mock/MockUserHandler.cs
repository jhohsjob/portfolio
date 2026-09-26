using System.Threading.Tasks;


public class MockUserHandler
{
    private readonly MockUserService _userService;

    public MockUserHandler(MockUserService user)
    {
        _userService = user;
    }

#if DEBUG_MODE
    public Task<DebugAddUserExpResponse> DebugUserAddExpAsync()
    {
        return _userService.DebugUserAddExpAsync();
    }

    public Task<DebugAddGoldResponse> DebugAddGoldAsync()
    {
        return _userService.DebugAddGoldAsync();
    }
#endif
}