public class User
{
    private readonly Storage _storage;
    private readonly SaveService _saveService;

    private long _userId;

    private int _mercenaryId;
    public int mercenaryId => _mercenaryId;

    private int _currentStageId;
    public int currentStageId => _currentStageId;

    public User(Storage storage, SaveService saveService)
    {
        _storage = storage;
        _saveService = saveService;

        _userId = 0;
        _currentStageId = 0;
    }

    public void RunGame()
    {
        var data = _storage.data.player;

        _mercenaryId = data.mercenaryId;
        _currentStageId = data.currentStageId;
    }

    public void SetMercenary(int id)
    {
        _mercenaryId = id;

        _storage.data.player.mercenaryId = _mercenaryId;
        _saveService.RequestSave();
    }

    public void SetStage(int id)
    {
        _currentStageId = id;

        _storage.data.player.currentStageId = _currentStageId;
        _saveService.RequestSave();
    }
}
