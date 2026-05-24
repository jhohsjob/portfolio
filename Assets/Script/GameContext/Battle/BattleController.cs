using UnityEngine;


public interface IBattleState
{
    bool IsRunning();
}

public interface IBattleController
{
    void SetStatus(BattleStatus status);
}

public class BattleController : IBattleState, IBattleController
{
    private BattleStatus _status = BattleStatus.None;

    public void SetStatus(BattleStatus status)
    {
        _status = status;

        switch (_status)
        {
            case BattleStatus.Running:
                Time.timeScale = 1f;
                break;
            case BattleStatus.Paused:
                Time.timeScale = 0f;
                break;
        }

        EventHelper.Send(EventName.BattleStatus, this, _status);
    }

    public bool IsRunning()
    {
        return _status == BattleStatus.Running;
    }
}