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