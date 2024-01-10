public enum BattleStatus { None, Run, Pause, BattleOver }
public enum RoleType
{
    Player = 10000, 
    Enemy = 20000, 
    Projectile = 30000
}
public enum Team { None, Player, Enemy }

public enum DropItemType
{
    None,
    Element,
    Coin,
}

public enum ElementType
{
    Water,
    Forest,
    Fire
}