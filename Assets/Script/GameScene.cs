using System.Collections;
using UnityEngine;


public class GameScene : MonoBehaviour
{
    [SerializeField]
    private Map _map;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private PlayerCamera _playerCamera;

    [SerializeField]
    public Transform enemyContainer;
    [SerializeField]
    public Transform projectileContainer;

    private float GAME_START_TIME = 3f;

    private void Awake()
    {
    }

    void Start()
    {
        Init();
    }
    
    private void Init()
    {
        GameManager.instance.Init(this);
        SpawnManager.instance.Init();
        EnemyManager.instance.Init(this);

        _map.Init();

        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(GAME_START_TIME);

        EventHelper.Send(EventName.GameStatus, this, Enums.GameStatus.Run);
    }
}