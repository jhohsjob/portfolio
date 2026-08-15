using System;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class SkillData
{
    protected SkillDefinition _data;

    public int id => _data.id;
    public string localTable => LocalTable.SkillTable;
    public string name => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetNameKey());
    public string description => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetDescKey());
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
