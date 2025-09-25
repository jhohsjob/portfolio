using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor<Mercenary, MercenaryData>
{
    private Vector2 _moveDirection = Vector2.zero;

    private Dictionary<ElementType, int> _elements = new();

    // temp
    private List<SkillData> _tempSkillDatas = new();

    private int _gainGold;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Player;
    }

    void Update()
    {
        bool isControl = (_moveDirection != Vector2.zero && BattleManager.instance.battleStatus == BattleStatus.Run);
        if (isControl == true)
        {
            Move();
        }
    }

    public override void Init(Mercenary role)
    {
        base.Init(role);

        _body.cbTriggerEnter += OnBodyTriggerEnter;

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

        _gainGold = 0;
    }

    public override void Enter(object data = null)
    {
        base.Enter();

        _hp.Init(_role.maxHP);
    }

    protected override void Move()
    {
        if (_moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
            _point.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        transform.Translate(_moveDirection * _role.moveSpeed * Time.deltaTime);

        var pos = transform.position;
        var mapBounds = BattleManager.instance.mapBounds;

        pos.x = Mathf.Clamp(pos.x, mapBounds.min.x + 0.5f, mapBounds.max.x - 0.5f);
        pos.y = Mathf.Clamp(pos.y, mapBounds.min.y + 0.6f, mapBounds.max.y - 0.6f);

        transform.position = pos;
    }

    protected override void Die()
    {
        Debug.Log("game over");

        BattleOver();

        _animator.SetBool("Dead", true);

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

    public void AddGold(int gold)
    {
        _gainGold += gold;

        EventHelper.Send(EventName.AddGold, this, _gainGold.ToString());
    }

    private void BattleOver()
    {
        Client.user.ChangeGold(_gainGold);
    }

    // PlayerInput Send Messages
    public void OnMove(InputValue value)
    {
        _moveDirection = value.Get<Vector2>();

        _animator.SetFloat("Speed", _moveDirection == Vector2.zero ? 0f : 1f);
        
        if (_moveDirection.x != 0f && _sprite.flipX != _moveDirection.x < 0f)
        {
            _sprite.flipX = _moveDirection.x < 0f;
        }

        // Debug.Log(_moveDirection);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        Debug.Log("OnTriggerEnter player : " + other);

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
