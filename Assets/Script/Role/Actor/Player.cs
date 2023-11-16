using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor
{
    public override Team team { get; protected set; } = Team.Player;

    [SerializeField]
    private Body _body;
    
    private Vector3 _moveDirection = Vector3.zero;

    // temp
    private List<SkillData> _tempSkillDatas = new List<SkillData>();

    protected override void Awake()
    {
        base.Awake();

        _body.cbTriggerEnter += OnBodyTriggerEnter;
}

    void Update()
    {
        bool isControl = (_moveDirection != Vector3.zero && GameManager.instance.gameStatus == GameStatus.Run);
        if (isControl == true)
        {
            Move();
        }
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

    public override void Init(RoleData roleData)
    {
        var data = roleData as PlayerData;
        if (data == null)
        {
            return;
        }

        base.Init(roleData);

        // temp
        _tempSkillDatas.Add(GameTable.GetSkillData(401001));

        foreach (var tempSkillData in _tempSkillDatas)
        {
            var type = Type.GetType(tempSkillData.behaviourName);
            var skill = (new GameObject(tempSkillData.skillName)).AddComponent(type) as Skill;
            skill.transform.SetParent(transform, false);
            skill.Init(this, tempSkillData);
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

        EventHelper.Send(EventName.GameStatus, this, GameStatus.GameOver);

        // base.Die();
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
