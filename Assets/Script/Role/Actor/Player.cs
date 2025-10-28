using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor<Mercenary, MercenaryData>
{
    private Vector2 _moveDirection = Vector2.zero;
    private Vector2 _lookDirection = Vector2.right;

    private Dictionary<ElementType, int> _elements = new();

    private DashController _dash;
    public DashController dash => _dash;

    // temp
    private List<SkillData> _tempSkillDatas = new();

    private int _gainGold;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Player;

        _dash = new DashController();

        EventHelper.AddEventListener(EventName.ClickBtnDash, OnClickBtnDash);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ClickBtnDash, OnClickBtnDash);
    }

    void Update()
    {
        _dash.Update(Time.deltaTime);

        if (_state.HasState(ActorState.Move))
        {
            Move();
        }
    }

    public override void Init(Mercenary role)
    {
        base.Init(role);

        _body.cbTriggerEnter += OnBodyTriggerEnter;

        // temp
        _tempSkillDatas.Add(role.skillData);

        foreach (var tempSkillData in _tempSkillDatas)
        {
            var skill = (new GameObject(tempSkillData.skillName)).AddComponent(tempSkillData.behaviourType) as Skill;
            skill.transform.SetParent(transform, false);
            skill.Init(this, tempSkillData);
        }

        _elements.Clear();

        _dash.Init(_role.dashSpeed, _role.dashCount, _role.dashCooldown, _body.sprite);

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
        if (BattleManager.instance.IsBattleRun() == false)
        {
            return;
        }

        _moveDirection = value.Get<Vector2>();
        if (_moveDirection != Vector2.zero)
        {
            _lookDirection = _moveDirection;
        }

        _animator.SetFloat("Speed", _moveDirection == Vector2.zero ? 0f : 1f);

        if (_moveDirection.x != 0f)
        {
            _body.FlipX(_moveDirection.x);
        }

        _state.SetState(_moveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);

        // Debug.Log(_moveDirection);
    }

    // PlayerInput Send Messages
    public void OnDash(InputValue value)
    {
        if (BattleManager.instance.IsBattleRun() == false ||
            _dash.canDash == false)
        {
            return;
        }

        _state.SetState(ActorState.Dash);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        // Debug.Log("OnTriggerEnter player : " + other);

        // 피격시 로직
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is Enemy enemy)
            {
                _state.SetState(ActorState.Die);
            }
            else if (body.actor is ICollectableDropItem collectableDropItem)
            {
                collectableDropItem.OnCollectedByPlayer(this);
            }
        }
    }

    protected override void OnStateChanged(ActorState state)
    {
        if (state == ActorState.Dash)
        {
            _collider.enabled = false;
            _dash.Dash(transform, _lookDirection, () =>
            {
                _collider.enabled = true;
                _state.SetState(_moveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
            });
        }
        else if (state == ActorState.Die)
        {
            Die();
        }

        Debug.Log("state change : " + state);
    }

    private void OnClickBtnDash(object sender, object data)
    {
        if (_dash.canDash == false)
        {
            return;
        }

        _state.SetState(ActorState.Dash);
    }
}