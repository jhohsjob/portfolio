using UnityEngine;


public class DropItem : Role
{
    public override Team team { get; protected set; } = Team.None;

    public DropItemType type { get; protected set; }

    private Body _body;

    private void Awake()
    {
        _body = transform.GetComponentInChildren<Body>();
        _body.cbTriggerEnter += OnBodyTriggerEnter;
    }

    void Update()
    {
    }

    public override void Init(RoleData roleData)
    {
        var data = roleData as DropItemData;
        if (data == null)
        {
            return;
        }

        type = data.type;

        base.Init(roleData);
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        var dropItemData = _roleData as DropItemData;
    }

    protected override void Die()
    {
        BattleManager.instance.roleManager.Retrieve(this);

        base.Die();
    }

    private void OnBodyTriggerEnter(Body other)
    {
        if (other.TryGetComponent(out Body body))
        {
            if (body.role is Player player)
            {
                player.GetDropItem(this);

                Die();
            }
        }
    }
}
