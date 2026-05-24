using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerContext
{
    public IBattleState battleState;
    public IAssetLoader assetLoader;
    public IActorSpawner actorSpawner;
    public Func<Vector3, Vector3, float, float, Transform> GetNearestEnemy;
}

public class Player : Actor<Mercenary, MercenaryDefinition>
{
    private PlayerContext _context;

    private ElementController _element;
    public ElementController element => _element;

    private IInputSource _inputSource;
    public IInputSource inputSource => _inputSource;

    private DashController _dash;
    public DashController dash => _dash;

    public override float moveSpeed => _role.moveSpeed + _additionalMoveSpeed;
    private float _additionalMoveSpeed;

    // temp
    private List<SkillData> _tempSkillDatas = new();

    public event Action<int> onGoldCollected;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Player;

        _element = new ElementController();
        _dash = new DashController();
        _inputSource = new PlayerInputSource();
    }

    private void OnDestroy()
    {
        _element.onLevelup -= OnElementLevelUp;
    }

    protected override void Update()
    {
        _dash?.Update(Time.deltaTime);

        _inputSource?.UpdateInput();

        base.Update();
    }

    public void InitDependencies(PlayerContext context)
    {
        _context = context;
        _dash.InitDependencies(this, context.assetLoader);
    }

    public override void Init(Mercenary role)
    {
        base.Init(role);

        // temp
        CreateSkill();

        _element.Init();
        _dash.Init(_role.dashSpeed, _role.dashCount, _role.dashCooldown, _body.sprite);

        Bind();
    }

    protected override void Die()
    {
        _animator.SetBool("Dead", true);

        base.Die();
    }

    private void Bind()
    {
        _body.OnTriggerEntered += OnBodyTriggerEnter;
        _element.onLevelup += OnElementLevelUp;
    }

    private void CreateSkill()
    {
        _tempSkillDatas.Add(role.skillData);

        foreach (var tempSkillData in _tempSkillDatas)
        {
            var skill = (new GameObject(tempSkillData.skillName)).AddComponent(tempSkillData.behaviourType) as Skill;
            skill.transform.SetParent(transform, false);
            skill.Init(this, tempSkillData, new SkillContext
            {
                battleState = _context.battleState,
                actorSpawner = _context.actorSpawner,
                GetProjectile = id => ProjectileManager.instance.GetProjectileById(id),
                GetNearestEnemy = _context.GetNearestEnemy
            });
        }
    }

    public void AddGold(int gold)
    {
        onGoldCollected?.Invoke(gold);
    }

    public void HandleJoystickAction(Vector2 value)
    {
        if (_inputSource is PlayerInputSource input)
        {
            input.OnMove(value);
        }

        _state.SetState(_inputSource.MoveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
    }

    public void HandleDashAction()
    {
        _dash.TryDash();
    }

    // PlayerInput Send Messages
    public void OnMove(InputValue value)
    {
        if (_context.battleState.IsRunning() == false)
        {
            return;
        }

        if (_inputSource is PlayerInputSource input)
        {
            input.OnMove(value.Get<Vector2>());
        }

        _state.SetState(_inputSource.MoveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
        //Debug.Log(_state.current);
    }

    // PlayerInput Send Messages
    public void OnDash(InputValue value)
    {
        _dash.TryDash();
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
            _dash.Dash(transform, _inputSource.LookDirection, () =>
            {
                _collider.enabled = true;
                _state.SetState(_inputSource.MoveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
            });
        }
        else if (state == ActorState.Die)
        {
            Die();
        }
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
        _additionalMoveSpeed += 0.5f;
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
}