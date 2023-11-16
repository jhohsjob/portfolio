using System.Collections;
using TMPro;
using UnityEngine;


public class UIMain : MonoBehaviour
{
    [SerializeField]
    private GameObject _panelStartDirection;
    [SerializeField]
    private GameObject _panelGameMain;

    [SerializeField]
    private TextMeshProUGUI _txtDirection;
    [SerializeField]
    private TextMeshProUGUI _txtKill;

    [SerializeField]
    private HPBarController _hpBarContainer;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.GameStatus, OnGameStatus);

        _panelStartDirection.SetActive(true);
        _panelGameMain.SetActive(false);
    }

    private void Update()
    {
        _txtKill.text = GameManager.instance.enemyManager.dieCount.ToString();
    }

    private IEnumerator StartDirection()
    {
        _txtDirection.text = "START";

        yield return new WaitForSeconds(1f);

        _panelStartDirection.SetActive(false);
        _panelGameMain.SetActive(true);
    }

    private void GameOverDirection()
    {
        _txtDirection.text = "YOU DIE";

        _panelStartDirection.SetActive(true);
        _panelGameMain.SetActive(false);
    }

    private void OnGameStatus(object sender, object data)
    {
        if (data == null || (data is GameStatus) == false)
        {
            return;
        }

        switch ((GameStatus)data)
        {
            case GameStatus.Run:
                StartCoroutine(StartDirection());
                break;

            case GameStatus.GameOver:
                GameOverDirection();
                break;
        }
    }
}
