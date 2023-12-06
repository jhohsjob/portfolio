using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyScene : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name);

        EventHelper.AddEventListener(EventName.BattleSceneMove, OnBattleSceneMove);
        
        Init();
    }

    private void Init()
    {
        GameTable.Load();
    }

    public void OnBattleSceneMove(object sender, object data)
    {
        if (data is MapInfoData == false)
        {
            return;
        }

        StartCoroutine(coBattleSceneMove(data as MapInfoData));
    }

    private IEnumerator coBattleSceneMove(MapInfoData mapInfoData)
    {
        yield return null;
        UIManager.instance.PopupClear();

        yield return null;
        var initData = new GameManagerInitData();
        initData.mapInfoData = mapInfoData;
        GameManager.instance.Init(initData);

        yield return null;
        var op = SceneManager.LoadSceneAsync("BattleScene");
        //op.allowSceneActivation = false;
        
        while (op.isDone == false)
        {
            Debug.Log(op.progress);
            
            yield return null;
        }
    }

    public void OnClickStart()
    {
        UIManager.instance.ShowPopup(PopupName.UISelectMap);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
