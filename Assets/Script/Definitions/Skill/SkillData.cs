using System;
using UnityEngine;


public class SkillData
{
    protected SkillDefinition _data;

    public int id => _data.id;
    public LocalizationTable localTable => LocalizationTable.SkillTable;
    public string name => LocalManager.GetLocalizedString(localTable, _data.GetNameKey());
    public string description => LocalManager.GetLocalizedString(localTable, _data.GetDescKey());
    public Sprite icon => _data.icon;

    public int fireCount => _data.fireCount;
    public int multiShotCount => _data.multiShotCount;
    public float spreadAngle => _data.spreadAngle;
    public float shotDelay => _data.shotDelay;
    public float reloadTime => _data.reloadTime;
    public ProjectileDefinition[] projectileData => _data.projectileData;

    public Type behaviourType => _data.behaviourType;

    public bool lastData;

    public SkillData(SkillDefinition data)
    {
        _data = data;
        lastData = false;
    }
}
