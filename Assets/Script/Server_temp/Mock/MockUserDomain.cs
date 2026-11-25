using System.Collections.Generic;
using System.Threading.Tasks;


public class MockUserDomain
{
    private readonly UserStorage _userStorage;
    private UserLevelSystem _levelSystem;

    public int level => _userStorage.pvo.level;
    public int gold => _userStorage.pvo.gold;
    public int gem => _userStorage.pvo.gem;

    public MockUserDomain(IAssetService assetService)
    {
        _userStorage = new UserStorage(assetService);
        _levelSystem = new UserLevelSystem();
    }

    public void SetupLevels(List<UserLevelData> userLevels)
    {
        _levelSystem.Setup(userLevels);
    }

    public async Task<UserPVO> InitializeAsync()
    {
        await _userStorage.LoadAsync();

        _levelSystem.Initialize(_userStorage.pvo.exp, _userStorage.pvo.level);

        return GetUser();
    }

    public Task<bool> AddExpAsync(int amount)
    {
        if (amount <= 0)
        {
            return Task.FromResult(false);
        }

        _userStorage.pvo.exp += amount;

        return Task.FromResult(true);
    }

    public async Task<bool> ChangeGold(int amount)
    {
        if (amount == 0)
        {
            return false;
        }

        _userStorage.pvo.gold += amount;

        await _userStorage.SaveAsync(_userStorage.pvo);

        return true;
    }

    public async Task<bool> ChangeGem(int amount)
    {
        if (amount == 0)
        {
            return false;
        }

        _userStorage.pvo.gem += amount;

        await _userStorage.SaveAsync(_userStorage.pvo);

        return true;
    }

    // default function
    public UserPVO GetUser()
    {
        return new UserPVO(_userStorage.pvo);
    }

#if DEBUG_MODE
    public async Task<DebugAddUserExpResponse> DebugUserAddExpAsync()
    {
        //
        int debugExpAmount = 1000;

        DebugAddUserExpResponse response = new();
        response.prevLevel = _userStorage.pvo.level;

        _levelSystem.AddExp(debugExpAmount);

        _userStorage.pvo.exp = _levelSystem.exp;
        _userStorage.pvo.level = _levelSystem.level;

        await _userStorage.SaveAsync(_userStorage.pvo);

        response.currentLevel = _userStorage.pvo.level;
        response.userPVO = GetUser();
        
        return response;
    }

    public async Task<DebugAddGoldResponse> DebugAddGoldAsync()
    {
        int debugGoldAmount = 1000000;

        DebugAddGoldResponse response = new();
        response.prevGold = _userStorage.pvo.gold;

        _userStorage.pvo.gold += debugGoldAmount;
        
        await _userStorage.SaveAsync(_userStorage.pvo);
        
        response.currentGold = _userStorage.pvo.gold;
        response.userPVO = GetUser();
        
        return response;
    }
#endif
}