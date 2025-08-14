using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor<Mercenary, MercenaryData>
{
    public override Team team { get; protected set; } = Team.Player;

    private Body _body;
    
    private Vector3 _moveDirection = Vector3.zero;

    private Dictionary<ElementType, int> _elements = new();

    // temp
    private List<SkillData> _tempSkillDatas = new();

    protected override void Awake()
    {
        base.Awake();

        _body = transform.GetComponentInChildren<Body>();
        _body.cbTriggerEnter += OnBodyTriggerEnter;
}

    void Update()
    {
        bool isControl = (_moveDirection != Vector3.zero && BattleManager.instance.battleStatus == BattleStatus.Run);
        if (isControl == true)
        {
            Move();
        }
    }
    public override void Init(Mercenary role)
    {
        base.Init(role);

        // temp
        _tempSkillDatas.Add(DataManager.GetSkillData(501001));

        foreach (var tempSkillData in _tempSkillDatas)
        {
            var type = Type.GetType(tempSkillData.behaviourName);
            var skill = (new GameObject(tempSkillData.skillName)).AddComponent(type) as Skill;
            skill.transform.SetParent(transform, false);
            skill.Init(this, tempSkillData);
        }

        _elements.Clear();
    }

    public override void Enter(object data = null)
    {
        base.Enter();

        _hp.Init(_role.maxHP);
    }

    protected override void Move()
    {
        transform.rotation = Quaternion.LookRotation(_moveDirection);
        transform.Translate(Vector3.forward * _role.moveSpeed * Time.deltaTime);

        var pos = transform.position;
        var mapSize = BattleManager.instance.mapSize;
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

    protected override void Die()
    {
        Debug.Log("game over");

        BattleManager.instance.SetBattleStatus(BattleStatus.BattleOver);
    }

    public void AddElement(ActorDIElement element)
    {
        if (element == null)
        {
            return;
        }

        if (_elements.ContainsKey(element.elementType) == false)
        {
            _elements.Add(element.elementType, 1);
        }
        else
        {
            _elements[element.elementType] += 1;
        }

        EventHelper.Send(EventName.AddElement, this, _elements);
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
            else if (body.actor is ICollectableDropItem collectableDropItem)
            {
                collectableDropItem.OnCollectedByPlayer(this);
            }
        }
    }
}
