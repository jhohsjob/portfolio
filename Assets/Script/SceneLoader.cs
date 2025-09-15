using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneLoader
{
    public static void LoadBattleScene(MapInfoData mapInfoData)
    {
        IEnumeratorTool.instance.StartCoroutine(coLoadBattleScene(mapInfoData));
    }

    private static IEnumerator coLoadBattleScene(MapInfoData mapInfoData)
    {
        yield return null;
        PopupManager.CloseAllPopup();

        yield return null;
        var initData = new BattleManagerInitData();
        initData.mapInfoData = mapInfoData;
        BattleManager.instance.Init(initData);

        yield return null;
        var op = SceneManager.LoadSceneAsync("BattleScene");
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
        var op = SceneManager.LoadSceneAsync("LobbyScene");
        //op.allowSceneActivation = false;

        while (op.isDone == false)
        {
            Debug.Log(op.progress);

            yield return null;
        }
        
        yield return null;
        BattleManager.instance.DestroySingleton();
    }
}
