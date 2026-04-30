using System;
using UnityEngine;
using UnityEngine.Localization.Settings;


public abstract class Role<TData> : RoleBase where TData : RoleData
{
    protected TData _data;
    public TData data => _data;

    public RoleType roleType => _data.roleType;
    public int id => _data.id;
    public virtual string localTable => LocalTable.None;
    public string name => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetNameKey());
    public string description => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetDescKey());

    public Type behaviourType => _data.behaviourType;

    public string resourcePath => _data.resourcePath;
    public Vector3 resourceOffset => _data.resourceOffset;

    public int atk => 0;
    public float maxHP => _data.maxHP;
    public float moveSpeed => _data.moveSpeed;

    public GameObject original;

    public Role(TData data)
    {
        _data = data.DeepCopy();

        if (_data.body == null)
        {
            Client.asset.LoadAsset<GameObject>(_data.resourcePath, (task) =>
            {
                original = task.GetAsset<GameObject>();
            });
        }
        else
        {
            original = _data.body;
        }
    }
}