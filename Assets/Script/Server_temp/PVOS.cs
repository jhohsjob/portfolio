public class UserPVO
{
    public long id;
    public string name;
    public int level;
    public int exp;
    public int gold;
    public int gem;
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
        gem = other.gem;
        lastPlayMercenaryId = other.lastPlayMercenaryId;
        lastPlayStageId = other.lastPlayStageId;
    }
}


public class MercenaryPVO
{
    public int id;
    public bool isOwned;
    public int level;
    public int useLevelUpGold;
    public int grade;
    public int shardCount;

    public MercenaryPVO() { }

    public MercenaryPVO(MercenaryPVO other)
    {
        id = other.id;
        isOwned = other.isOwned;
        level = other.level;
        useLevelUpGold = other.useLevelUpGold;
        grade = other.grade;
        shardCount = other.shardCount;
    }
}


public class ProductPVO
{
    public int id;
    public int purchaseCount;
    public int totalPurchaseCount;
    public long lastPurchaseTime;
    public long lastResetTime;

    public ProductPVO() { }

    public ProductPVO(ProductPVO other)
    {
        id = other.id;
        purchaseCount = other.purchaseCount;
        lastPurchaseTime = other.lastPurchaseTime;
        lastResetTime = other.lastResetTime;
    }
}

public class GachaPVO
{
    public int id;
    public int gachaCount;
    public GachaPVO() { }

    public GachaPVO(GachaPVO other)
    {
        id = other.id;
        gachaCount = other.gachaCount;
    }
}