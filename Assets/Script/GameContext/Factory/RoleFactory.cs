using System;


public class RoleFactory
{
    public RoleFactory()
    {
    }

    public RoleBase Create(RoleDefinition data)
    {
        switch (data)
        {
            case MercenaryDefinition mercenaryData:
                return new Mercenary(mercenaryData);

            case MonsterDefinition monsterData:
                return new Monster(monsterData);

            case ProjectileDefinition projectileData:
                return new Projectile(projectileData);

            case DropItemDefinition dropItemData:
                return DropItemFactory.Create((DropItemDefinition)data);

            default:
                throw new Exception($"Unknown RoleData type: {data.GetType()}");
        }
    }
}
