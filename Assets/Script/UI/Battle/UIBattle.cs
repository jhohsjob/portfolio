using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattle : MonoBehaviour
{
    public enum PanelMode { Start, Main, End }

    [SerializeField]
    private UIBattleTop _uitop;
    [SerializeField]
    private RectTransform _topRect;
    [SerializeField]
    private GameObject _panelStartDirection;
    [SerializeField]
    private GameObject _panelBattleMain;
    [SerializeField]
    private GameObject _panelBattleEnd;

    [SerializeField]
    private TextMeshProUGUI _txtDirection;

    [SerializeField]
    private HPBarController _hpBarController;

    [SerializeField]
    private UIJoystick _uiJoystick;

    [SerializeField]
    private UIDash _uiDash;

    [SerializeField]
    private Button _btnLobby;

    public UIBattleTop TopView => _uitop;
    public UIJoystick JoystickView => _uiJoystick;
    public UIDash DashView => _uiDash;
    public HPBarController hpBarController => _hpBarController;

    public event Action onClickLobby;

    private void Awake()
    {
        _btnLobby.onClick.AddListener(() =>
        {
            onClickLobby?.Invoke();
        });
    }

    public IEnumerator StartDirection()
    {
        _txtDirection.text = "START";

        yield return new WaitForSeconds(1f);

        PanelChange(PanelMode.Main);
    }

    public void BattleWinDirection()
    {
        _txtDirection.text = "YOU Win";

        PanelChange(PanelMode.End);
    }

    public void BattleLoseDirection()
    {
        _txtDirection.text = "YOU DIE";

        PanelChange(PanelMode.End);
    }

    public void PanelChange(PanelMode mode)
    {
        switch (mode)
        {
            case PanelMode.Start:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.SetActive(false);
                _panelBattleEnd.SetActive(false);
                break;

            case PanelMode.Main:
                _panelStartDirection.SetActive(false);
                _panelBattleMain.SetActive(true);
                _panelBattleEnd.SetActive(false);
                break;

            case PanelMode.End:
                _panelStartDirection.SetActive(true);
                _panelBattleMain.SetActive(false);
                _panelBattleEnd.SetActive(true);
                break;
        }
    }

    public float GetTopUIHeight()
    {
        float screenHeight = Screen.height;
        float camHeight = Camera.main.orthographicSize * 2f;

        float uiRatio = _topRect.rect.height / screenHeight;

        return camHeight * uiRatio;
    }
}