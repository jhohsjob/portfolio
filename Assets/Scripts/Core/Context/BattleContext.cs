using UnityEngine;


public interface IBattleEnvironment
{
    IBattleState state { get; }
    IActorSpawner spawner { get; }
    Projectile GetProjectile(int id);
    Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float radius, float angle);
}

public sealed class BattleContext : IBattleEnvironment
{
    public sealed class Deps
    {
        public BattleRequest request;
        public IBattleState state;
        public BattleRewardRuntime reward;
        public ActorWorld world;
        public ProjectileCatalog projectileCatalog;
    }


    private readonly Deps _deps;

    public Stage stage => _deps.request.stage;
    public Mercenary mercenary => _deps.request.mercenary;

    public IBattleState state => _deps.state;
    public BattleRewardRuntime reward => _deps.reward;
    public IActorSpawner spawner => _deps.world.actorSpawner;

    public BattleContext(Deps deps)
    {
        _deps = deps;
    }

    public Projectile GetProjectile(int id) => _deps.projectileCatalog.GetProjectileById(id);
    public Transform GetNearestEnemy(Vector3 o, Vector3 f, float r, float a) => _deps.world.GetNearestEnemy(o, f, r, a);

    public int GetNextActorID(int roleId) => _deps.world.GetNextActorID(roleId);
    public int GetNextOrderInLayer(RoleType type) => _deps.world.GetNextOrderInLayer(type);
    public Transform GetMapOriginal() => _deps.world.GetMapOriginal();
    public void HandleGoldCollected(int gold) => reward.AddGold(gold);
}