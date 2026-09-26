using System.Threading.Tasks;


public class User
{
    private IGameServer _gameServer;

    private UserPVO _pvo;

    public long id => _pvo.id;
    public string name => _pvo.name;

    public int level => _pvo.level;
    public int exp => _pvo.exp;

    public int gold => _pvo.gold;

    public int lastPlayMercenaryId => _pvo.lastPlayMercenaryId;

    public int lastPlayStageId => _pvo.lastPlayStageId;

    public User(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }

    public void InitializeServer(UserPVO pvo)
    {
        ApplyPVO(pvo);
    }

    public void ApplyPVO(UserPVO pvo)
    {
        _pvo = new UserPVO(pvo);

        EventHelper.Send(EventName.UpdateUserPvo, this);
    }

    public void RunGame()
    {
    }

    public void SetMercenary(int id)
    {
        //_mercenaryId = id;

        //_storage.data.player.mercenaryId = _mercenaryId;
        //_saveService.RequestSave();
    }

    public void SetStage(int id)
    {
        //_currentStageId = id;

        //_storage.data.player.currentStageId = _currentStageId;
        //_saveService.RequestSave();
    }

    // temp
    public void SetGold(int value)
    {
        _pvo.gold = value;
    }

#if DEBUG_MODE
    public async Task DebugAddExp()
    {
        var request = new DebugAddUserExpRequest();

        var response = await _gameServer.DebugUserAddExpAsync();

        ApplyPVO(response.userPVO);
    }

    public async Task DebugAddGold()
    {
        var request = new DebugAddGoldRequest();

        var response = await _gameServer.DebugAddGoldAsync();

        ApplyPVO(response.userPVO);
    }
#endif
}
