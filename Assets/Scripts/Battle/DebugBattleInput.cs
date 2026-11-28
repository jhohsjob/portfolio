using UnityEngine;
using UnityEngine.InputSystem;


public class DebugBattleInput : MonoBehaviour
{
    public static bool debugEnemyPause { get; private set; }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            debugEnemyPause = !debugEnemyPause;
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            EventHelper.Send(EventName.DebugStageClear, this);
        }
    }
}