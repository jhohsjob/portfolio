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

        var dashView = gameObject.AddComponent<ActorDashView>();
        _view.AddView(dashView);
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
        _dash.InitDependencies(this);
    }

    public override void Init(Mercenary role)
    {
        base.Init(role);

        _context.assetLoader.LoadMaterial("DashMat", mat =>
        {
            var dashView = _view.GetView<ActorDashView>();
            dashView.SetMaterial(mat);
        });

        // temp
        CreateSkill();

        _element.Init();
        _dash.Init(_role.dashSpeed, _role.dashCount, _role.dashCooldown);
    }

    protected override void Die()
    {
        _view.DeadAnimation();

        base.Die();
    }

    protected override void Bind()
    {
        base.Bind();

        _element.onLevelup += OnElementLevelUp;
    }

    protected override void Unbind()
    {
        base.Unbind();

        _element.onLevelup -= OnElementLevelUp;
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

    protected override void OnBodyTriggerEnter(Body other)
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
            var result = _dash.Dash(transform, _inputSource.LookDirection, () =>
            {
                _view.SetCollider(true);
                _state.SetState(_inputSource.MoveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
            });
            if (result == true)
            {
                _view.SetCollider(false);
                _view.GetView<ActorDashView>().PlayDashEffect(_inputSource.LookDirection, _role.dashSpeed);
            }
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
            case ElementType.Nature:
                HandleNatureLevelUp();
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

    private void HandleNatureLevelUp()
    {
        _hp.OnElementLevelUp();
    }

    private void HandleFireLevelUp()
    {
        // todo : skill update
    }
}