using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattle : MonoBehaviour
{
    private enum PanelMode { start, run, over }

    [SerializeField]
    private GameObject _panelStartDirection;
    [SerializeField]
    private GameObject _panelBattleMain;
    [SerializeField]
    private GameObject _panelBattleOver;

    [SerializeField]
    private TextMeshProUGUI _txtDirection;
    [SerializeField]
    private TextMeshProUGUI _txtKill;

    [SerializeField]
    private HPBarController _hpBarContainer;

    [SerializeField]
    private Button _btnPause;
    [SerializeField]
    private Button _btnLobby;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);

        PanelChange(PanelMode.start);

        _btnPause.onClick.AddListener(OnClickPause);
        _btnLobby.onClick.AddListener(OnClickLobby);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    private void Update()
    {
        if (BattleManager.instance.battleStatus == BattleStatus.Run)
        {
            _txtKill.text = BattleManager.instance.enemyManager.dieCount.ToString();
        }
    }

    private IEnumerator StartDirection()
    {
        _txtDirection.text = "START";

        yield return new WaitForSeconds(1f);

        PanelChange(PanelMode.run);
    }

    private void BattleOverDirection()
    {
        _txtDirection.text = "YOU DIE";

        PanelChange(PanelMode.over);
    }

    private void PanelChange(PanelMode mode)
    {
        switch (mode)
        {
            case PanelMode.start:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.SetActive(false);
                _panelBattleOver.SetActive(false);
                break;

            case PanelMode.run:
                _panelStartDirection.SetActive(false);
                _panelBattleMain.SetActive(true);
                _panelBattleOver.SetActive(false);
                break;

            case PanelMode.over:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.SetActive(false);
                _panelBattleOver.SetActive(true);
                break;
        }
    }

    private void OnClickPause()
    {
        BattleManager.instance.SetBattleStatus(BattleStatus.Pause);

        UIManager.instance.ShowPopup(PopupName.UIPause);
    }

    private void OnClickLobby()
    {
        SceneLoader.LoadLobbyScene();
    }

    private void OnBattleStatus(object sender, object data)
    {
        if (data == null || (data is BattleStatus) == false)
        {
            return;
        }

        switch ((BattleStatus)data)
        {
            case BattleStatus.Run:
                StartCoroutine(StartDirection());
                break;

            case BattleStatus.BattleOver:
                BattleOverDirection();
                break;
        }
    }
}
