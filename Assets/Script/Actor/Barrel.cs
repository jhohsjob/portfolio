using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Actor _actor;

    [SerializeField]
    private Transform _point;

    private float START_SHOOT_TIME = 0f;
    [SerializeField]
    private float SHOOT_TIME = 1f;
    private float _shootTimer = 0f;
    private int _shootNumber = 1;
    private int _shootCount = 0;


    private void Awake()
    {
    }

    void Update()
    {
        if (GameManager.instance.gameStatus == Enums.GameStatus.Run)
        {
            _shootTimer += Time.deltaTime;

            if (_shootTimer >= SHOOT_TIME)
            {
                _shootTimer = 0f;
                _shootCount++;

                Shoot();
            }
        }
    }

    public void Init(Actor actor)
    {
        _actor = actor;
    }

    public void Shoot()
    {
        var obj = SpawnManager.instance.Spawn(Enums.SpawnType.Projectile, _shootNumber, GameManager.instance.gameScene.projectileContainer);
        var projectile = obj.GetComponent<Projectile>();
        projectile?.Init(_actor, _point);
    }
}
