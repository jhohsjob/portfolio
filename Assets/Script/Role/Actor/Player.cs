using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : Actor<Mercenary, MercenaryData>
{
    private PlayerMoveController _move;

    private ElementController _element;
    public ElementController element => _element;

    private DashController _dash;
    public DashController dash => _dash;

    // temp
    private List<SkillData> _tempSkillDatas = new();

    private int _gainGold;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Player;

        _move = new PlayerMoveController();
        _element = new ElementController();
        _dash = new DashController();

        EventHelper.AddEventListener(EventName.ClickDash, OnClickDash);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ClickDash, OnClickDash);

        _element.cbLevelup -= OnElementLevelUp;
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

        _move.Init(this);
        _element.Init(this);
        _dash.Init(_role.dashSpeed, _role.dashCount, _role.dashCooldown, _body.sprite);

        _gainGold = 0;
    }

    public override void Enter(object data = null)
    {
        base.Enter();

        _element.cbLevelup += OnElementLevelUp;
    }

    protected override void Move()
    {
        _move.Move();
    }

    protected override void Die()
    {
        Debug.Log("game over");

        BattleOver();

        _animator.SetBool("Dead", true);

        BattleManager.instance.SetBattleStatus(BattleStatus.BattleOver);
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

    public void SetJoystick(Vector2 input)
    {
        _move.SetMoveDirection(input);
    }

    // PlayerInput Send Messages
    public void OnMove(InputValue value)
    {
        if (BattleManager.instance.IsBattleRun() == false)
        {
            return;
        }

        _move.SetMoveDirection(value.Get<Vector2>());

        // Debug.Log(_moveDirection);
    }

    // PlayerInput Send Messages
    public void OnDash(InputValue value)
    {
        if (BattleManager.instance.IsBattleRun() == false || _dash.canDash == false)
        {
            return;
        }

        _state.SetState(ActorState.Dash);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        // Debug.Log("OnTriggerEnter player : " + other);

        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is ICollectableDropItem collectableDropItem)
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
            _dash.Dash(transform, _move.lookDirection, () =>
            {
                _collider.enabled = true;
                _state.SetState(_move.moveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
            });
        }
        else if (state == ActorState.Die)
        {
            Die();
        }

        Debug.Log("state change : " + state);
    }

    private void OnElementLevelUp(ElementType type)
    {
        switch (type)
        {
            case ElementType.Water:
                HandleWaterLevelUp();
                break;
            case ElementType.Forest:
                HandleForestLevelUp();
                break;
            case ElementType.Fire:
                HandleFireLevelUp();
                break;
        }
    }
    private void HandleWaterLevelUp()
    {
        _move.OnElementLevelUp();
        _dash.OnElementLevelUp();
    }

    private void HandleForestLevelUp()
    {
        _hp.OnElementLevelUp();
    }

    private void HandleFireLevelUp()
    {
        // todo : skill update
    }

    private void OnClickDash(object sender, object data)
    {
        if (_dash.canDash == false)
        {
            return;
        }

        _state.SetState(ActorState.Dash);
    }
}