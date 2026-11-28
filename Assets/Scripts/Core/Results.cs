public enum MercenaryAcquireResult
{
    Success,
    Invalid,
    InvalidId,
    AlreadyOwned,
    NotEnoughGold
}

public enum MercenaryLevelUpResult
{
    Success,
    Invalid,
    InvalidId,
    NotOwned,
    MaxLevel,
    InvalidLevel,
    NotEnoughGold
}

public enum MercenaryLevelResetResult
{
    Success,
    Invalid,
    InvalidId,
    NotOwned,
    MaxLevel,
    InvalidLevel,
    NotEnoughGold
}

public enum MercenaryGradeUpResult
{
    Success,
    Invalid,
    InvalidId,
    NotOwned,
    MaxGrade,
    InvalidGrade,
    NotEnoughShard
}

public enum PurchaseResult
{
    Success,
    Invalid,
    InvalidId,
    NotEnoughGold,
    NotEnoughGem,
    PurchaseLimitExceeded,
}

public enum GachaResult
{
    Success,
    Invalid,
    InvalidId,
    InvalidDrawCount,
    NotEnoughGold,
    NotEnoughGem,
    NotEnoughTicket,
}