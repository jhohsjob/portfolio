public class User
{
    private long _userId;

    private int _currentGold;
    public int gold => _currentGold;

    private int _mercenaryId;
    public int mercenaryId => _mercenaryId;

    private int _currentStageId;
    public int currentStageId => _currentStageId;

    public User()
    {
        _userId = 0;
        _currentGold = 0;
        _currentStageId = 0;
    }

    public void RunGame()
    {
        var data = Client.storage.data.player;
        ChangeGold(data.gold, false);
        _mercenaryId = data.mercenaryId;
        _currentStageId = data.currentStageId;
    }

    public bool ChangeGold(int amount, bool saveGold = true)
    {
        int newGold = _currentGold + amount;

        if (newGold < 0)
        {
            return false;
        }

        _currentGold = newGold;
        if (saveGold == true)
        {
            SaveGold();
        }

        EventHelper.Send(EventName.ChangeGold, this);

        return true;
    }

    private void SaveGold()
    {
        Client.storage.data.player.gold = _currentGold;
        StorageSaveManager.RequestSave();
    }

    public void SetMercenary(int id)
    {
        _mercenaryId = id;

        SaveMercenary();
    }

    private void SaveMercenary()
    {
        Client.storage.data.player.mercenaryId = _mercenaryId;
        StorageSaveManager.RequestSave();
    }

    public void SetStage(int id)
    {
        _currentStageId = id;

        SaveStage();
    }

    private void SaveStage()
    {
        Client.storage.data.player.currentStageId = _currentStageId;
        StorageSaveManager.RequestSave();
    }
}
