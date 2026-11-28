using System.Threading.Tasks;


public class MockUserHandler
{
    private readonly MockUserDomain _userService;

    public MockUserHandler(MockUserDomain user)
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