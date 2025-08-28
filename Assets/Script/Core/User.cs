public class User : Singleton<User>
{
    private long _userId;

    private int _currentGold;
    public int gold => _currentGold;

    private int _mercenaryId;
    public int mercenaryId => _mercenaryId;

    public void Init()
    {
        _userId = 0;

        var data = Storage.Data.player;
        _currentGold = data.gold;
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

    public bool ChangeGold(int amount)
    {
        int newGold = _currentGold + amount;

        if (newGold < 0)
        {
            return false;
        }

        _currentGold = newGold;
        SaveGold();
        return true;
    }

    private void SaveMercenary()
    {
        Storage.Data.player.mercenaryId = _mercenaryId;
        Storage.Save(Storage.Data);
    }

    private void SaveGold()
    {
        Storage.Data.player.gold = _currentGold;
        Storage.Save(Storage.Data);
    }
}
