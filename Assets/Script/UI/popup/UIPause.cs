using UnityEngine;
using UnityEngine.UI;


public class UIPause : UIPopup
{
    [SerializeField]
    protected Button _btnOption;
    [SerializeField]
    protected Button _btnLobby;
    
    protected override void Awake()
    {
        base.Awake();

        _btnOption.onClick.AddListener(OnClickOption);
        _btnLobby.onClick.AddListener(OnClickLobby);
    }

    private void OnClickOption()
    {

    }

    private void OnClickLobby()
    {
        SceneLoader.LoadLobbyScene();
    }

    protected override void OnClickClose()
    {
        BattleManager.instance.SetBattleStatus(BattleStatus.Run);

        base.OnClickClose();
    }
}
