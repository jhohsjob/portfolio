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
    public UIMain uiMain;

    [SerializeField]
    public Transform actorContainer;

    public Player player => _player;

    private float GAME_START_TIME = 3f;

    private void Awake()
    {
        Debug.Log("GameScene.Awake");
    }

    void Start()
    {
        Debug.Log("GameScene.Start");

        Init();
    }
    
    private void Init()
    {
        GameManager.instance.SetGameScene(this);
        _player.Init(Resources.Load<PlayerData>("Data/Role/Actor/Player/Mercenary01"));
        _player.Enter();

        _map.Init();

        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(GAME_START_TIME);

        EventHelper.Send(EventName.GameStatus, this, GameStatus.Run);
    }

    public Vector3 GetRandomPos()
    {
        return _map.GetRandomPos();
    }
}