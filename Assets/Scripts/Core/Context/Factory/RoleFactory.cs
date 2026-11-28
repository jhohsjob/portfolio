using System;


public class RoleFactory
{
    public RoleFactory()
    {
    }

    public RoleBase Create(RoleDefinition def)
    {
        switch (def)
        {
            case MercenaryDefinition mercenaryData:
                return new Mercenary(mercenaryData);

            case MonsterDefinition monsterData:
                return new Monster(monsterData);

            case ProjectileDefinition projectileData:
                return new Projectile(projectileData);

            case DropItemDefinition dropItemData:
                return DropItemFactory.Create((DropItemDefinition)def);

            default:
                throw new Exception($"Unknown RoleData type: {def.GetType()}");
        }
    }
}
