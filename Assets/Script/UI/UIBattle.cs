using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattle : MonoBehaviour
{
    private enum PanelMode { Start, Main, End }

    [SerializeField]
    private RectTransform _topUI;
    [SerializeField]
    private GameObject _panelStartDirection;
    [SerializeField]
    private UIPanelBattleMain _panelBattleMain;
    [SerializeField]
    private GameObject _panelBattleEnd;

    [SerializeField]
    private TextMeshProUGUI _txtDirection;

    [SerializeField]
    private HPBarController _hpBarContainer;
    
    [SerializeField]
    private Button _btnLobby;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);

        PanelChange(PanelMode.Start);

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

        PanelChange(PanelMode.Main);
    }

    private void BattleWinDirection()
    {
        _txtDirection.text = "YOU Win";

        PanelChange(PanelMode.End);
    }

    private void BattleLoseDirection()
    {
        _txtDirection.text = "YOU DIE";

        PanelChange(PanelMode.End);
    }

    private void PanelChange(PanelMode mode)
    {
        switch (mode)
        {
            case PanelMode.Start:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.Hide();
                _panelBattleEnd.SetActive(false);
                break;

            case PanelMode.Main:
                _panelStartDirection.SetActive(false);
                _panelBattleMain.Show();
                _panelBattleEnd.SetActive(false);
                break;

            case PanelMode.End:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.Hide();
                _panelBattleEnd.SetActive(true);
                break;
        }
    }

    public float GetTopUIHeight()
    {
        float screenHeight = Screen.height;
        float camHeight = Camera.main.orthographicSize * 2f;

        float uiRatio = _topUI.rect.height / screenHeight;

        return camHeight * uiRatio;
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
            case BattleStatus.Running:
                StartCoroutine(StartDirection());
                break;

            case BattleStatus.Win:
                BattleWinDirection();
                break;

            case BattleStatus.Lose:
                BattleLoseDirection();
                break;
        }
    }
}
