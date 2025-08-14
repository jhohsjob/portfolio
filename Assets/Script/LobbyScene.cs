using System.Reflection;
using UnityEngine;


public class LobbyScene : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name);

        Init();
    }

    private void Init()
    {
        DataManager.Load();
        User.instance.Init();
    }
}
