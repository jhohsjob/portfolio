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
    private UIPanelBattleMain _panelBattleMain;
    [SerializeField]
    private GameObject _panelBattleOver;

    [SerializeField]
    private TextMeshProUGUI _txtDirection;

    [SerializeField]
    private HPBarController _hpBarContainer;
    
    [SerializeField]
    private Button _btnLobby;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);

        PanelChange(PanelMode.start);

        _btnLobby.onClick.AddListener(OnClickLobby);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
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
                _panelBattleMain.Hide();
                _panelBattleOver.SetActive(false);
                break;

            case PanelMode.run:
                _panelStartDirection.SetActive(false);
                _panelBattleMain.Show();
                _panelBattleOver.SetActive(false);
                break;

            case PanelMode.over:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.Hide();
                _panelBattleOver.SetActive(true);
                break;
        }
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
