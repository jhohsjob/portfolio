using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneLoader
{
    public static void LoadBattleScene()
    {
        IEnumeratorTool.instance.StartCoroutine(coLoadBattleScene());
    }

    private static IEnumerator coLoadBattleScene()
    {
        yield return null;
        PopupManager.CloseAllPopup();

        //yield return null;
        //BattleManager.instance.Init();

        yield return null;
        var op = SceneManager.LoadSceneAsync("03.BattleScene");
        //op.allowSceneActivation = false;

        while (op.isDone == false)
        {
            Debug.Log(op.progress);

            yield return null;
        }
    }

    public static void LoadLobbyScene()
    {
        IEnumeratorTool.instance.StartCoroutine(coLoadLobbyScene());
    }

    private static IEnumerator coLoadLobbyScene()
    {
        yield return null;
        PopupManager.CloseAllPopup();

        yield return null;
        var op = SceneManager.LoadSceneAsync("02.LobbyScene");
        //op.allowSceneActivation = false;

        while (op.isDone == false)
        {
            Debug.Log(op.progress);

            yield return null;
        }
        
        yield return null;
        BattleManager.instance.DestroySingleton();

        EventHelper.Send(EventName.ChangeGold);
    }
}
