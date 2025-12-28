public class User
{
    private long _userId;

    private int _currentGold;
    public int gold => _currentGold;

    private int _mercenaryId;
    public int mercenaryId => _mercenaryId;

    public User()
    {
        _userId = 0;
        _currentGold = 0;
    }

    public void RunGame()
    {
        var data = Client.storage.data.player;
        ChangeGold(data.gold, false);
        _mercenaryId = data.mercenaryId;
    }

    public void SetMercenary(int id)
    {
        _mercenaryId = id;

        SaveMercenary();
    }

    public Mercenary GetMercenary()
    {
        return MercenaryHander.instance.GetMercenaryById(_mercenaryId);
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

    private void SaveMercenary()
    {
        Client.storage.data.player.mercenaryId = _mercenaryId;
        StorageSaveManager.RequestSave();
    }

    private void SaveGold()
    {
        Client.storage.data.player.gold = _currentGold;
        StorageSaveManager.RequestSave();
    }
}
