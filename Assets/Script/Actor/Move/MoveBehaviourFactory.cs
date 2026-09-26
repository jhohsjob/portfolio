
public interface IMoveContext
{
}

public static class MoveBehaviourFactory
{
    public static IMoveBehaviour Create(ActorMoveType moveType)
    {
        return moveType switch
        {
            ActorMoveType.None => null,
            ActorMoveType.Straight => new StraightMove(),
            ActorMoveType.Orbit => new OrbitMove(),
            ActorMoveType.Homing => new HomingMove(),
            ActorMoveType.GroundZone => new GroundZoneMove(),
            ActorMoveType.ChaseTarget => new ChaseTargetMove(),
            ActorMoveType.Direction => new DirectionMove(),

            _ => null
        };
    }
}