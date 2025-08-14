using UnityEngine;

public class User : Singleton<User>
{
    private long _userId;

    private int _mercenaryId;
    public int mercenaryId => _mercenaryId;

    public void Init()
    {
        _userId = 0;

        var mercenary = MercenaryHander.instance.GetMercenaryByIndex(0);
        _mercenaryId = mercenary.id;
    }

    public void SetMercenary(int id)
    {
        _mercenaryId = id;
    }

    public Mercenary GetMercenary()
    {
        return MercenaryHander.instance.GetMercenaryById(_mercenaryId);
    }
}
