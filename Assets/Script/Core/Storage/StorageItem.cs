[System.Serializable]
public class GameSaveData
{
    public UserData player;
}

[System.Serializable]
public class UserData
{
    public int level;
    public int exp;
    public int gold;
    public int mercenaryId;

    public UserData(UserDefaultData data)
    {
        level = data.level;
        exp = data.exp;
        gold = data.gold;
        mercenaryId = data.mercenaryId;
    }
}