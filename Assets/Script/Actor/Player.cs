using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor
{
    public override Enums.Team team { get; set; } = Enums.Team.Player;
    protected override float _moveSpeed { get; set; } = 3f;
    
    [SerializeField]
    private Body _body;
    [SerializeField]
    private Barrel _barrel;

    private Vector3 _moveDirection;

    private void Awake()
    {
        _body.cbTriggerEnter += OnBodyTriggerEnter;
    }

    private void Start()
    {
        _barrel.Init(this);
    }

    void Update()
    {
        bool isControl = (_moveDirection != Vector3.zero && GameManager.instance.gameStatus == Enums.GameStatus.Run);
        if (isControl == true)
        {
            Move();
        }
    }

    protected override void Move()
    {
        transform.rotation = Quaternion.LookRotation(_moveDirection);
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);

        var pos = transform.position;
        var mapSize = GameManager.instance.mapSize;
        if (pos.x > mapSize.x - 0.5f)
        {
            pos.x = mapSize.x - 0.5f;
            transform.position = pos;
        }
        else if (pos.x < -mapSize.x + 0.5f)
        {
            pos.x = -mapSize.x + 0.5f;
            transform.position = pos;
        }

        if (pos.z > mapSize.z - 0.5f)
        {
            pos.z = mapSize.z - 0.5f;
            transform.position = pos;
        }
        else if (pos.z < -mapSize.z + 0.5f)
        {
            pos.z = -mapSize.z + 0.5f;
            transform.position = pos;
        }
    }

    public override void Die()
    {
        Debug.Log("game over");

        EventHelper.Send(EventName.GameStatus, this, Enums.GameStatus.GameOver);
    }

    // PlayerInput Send Messages
    public void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        if (input != null)
        {
            _moveDirection = new Vector3(input.x, 0f, input.y);

            // Debug.Log(_moveDirection);
        }
    }

    private void OnBodyTriggerEnter(Body other)
    {
        Debug.Log("OnTriggerEnter : " + other);

        // 피격시 로직
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is Enemy enemy)
            {
                Die();
            }
        }
    }
}
