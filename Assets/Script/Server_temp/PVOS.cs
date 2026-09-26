public class UserPVO
{
    public long id;
    public string name;
    public int level;
    public int exp;
    public int gold;
    public int lastPlayMercenaryId;
    public int lastPlayStageId;

    public UserPVO() { }

    public UserPVO(UserPVO other)
    {
        id = other.id;
        name = other.name;
        level = other.level;
        exp = other.exp;
        gold = other.gold;
        lastPlayMercenaryId = other.lastPlayMercenaryId;
        lastPlayStageId = other.lastPlayStageId;
    }
}


public class MercenaryPVO
{
    public int id;
    public bool isOwned;
    public int level;
    public int grade;
    public int useGold;

    public MercenaryPVO() { }

    public MercenaryPVO(MercenaryPVO other)
    {
        id = other.id;
        isOwned = other.isOwned;
        level = other.level;
        grade = other.grade;
        useGold = other.useGold;
    }
}
