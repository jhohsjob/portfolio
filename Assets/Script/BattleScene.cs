using System.Collections;
using UnityEngine;


public class BattleScene : MonoBehaviour
{
    [SerializeField]
    private Map _map;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private PlayerCamera _playerCamera;

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
        BattleManager.instance.SetBattleScene(this);
        var mercenary = Client.user.GetMercenary();
        _player.Init(mercenary);
        _player.Enter();

        _map.Init();

        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(GAME_START_TIME);

        BattleManager.instance.SetBattleStatus(BattleStatus.Run);
    }

    public Vector3 GetRandomPos()
    {
        return _map.GetRandomPos(_player.transform.position);
    }
}