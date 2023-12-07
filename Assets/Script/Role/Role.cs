using UnityEngine;


public abstract class Role : MonoBehaviour
{
    public int ID { get; private set; }

    protected RoleData _roleData { get; private set; }
    public int roleId { get; private set; }
    protected float _moveSpeed { get; set; }

    public virtual Team team { get; protected set; }
    
    public virtual void Init(RoleData roleData)
    {
        _roleData = roleData;

        roleId = _roleData.id;

        var body = Instantiate(Resources.Load<GameObject>(roleData.resourcePath), transform);
        body.transform.localPosition = roleData.resourceOffset;
    }

    private void SetID(int id)
    {
        ID = id;

        gameObject.name = _roleData.roleName + "_" + ID;
    }

    public virtual void Enter()
    {
        SetID(BattleManager.instance.roleManager.GetNextID(roleId));

        _moveSpeed = _roleData.moveSpeed;
    }

    protected abstract void Move();
    public virtual void Die()
    {
        SetID(0);
    }
}
