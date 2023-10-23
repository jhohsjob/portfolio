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

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.GameStatus, OnGameStatus);

        _panelStartDirection.SetActive(true);
        _panelGameMain.SetActive(false);
    }

    private void Update()
    {
        _txtKill.text = EnemyManager.instance.dieCount.ToString();
    }

    private void OnGameStatus(object sender, object data)
    {
        if (data == null || (data is Enums.GameStatus) == false)
        {
            return;
        }

        switch ((Enums.GameStatus)data)
        {
            case Enums.GameStatus.Run:
                StartCoroutine(StartDirection());
                break;

            case Enums.GameStatus.GameOver:
                GameOverDirection();
                break;
        }
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
}
