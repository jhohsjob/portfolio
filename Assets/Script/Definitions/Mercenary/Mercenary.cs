using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Mercenary : Role<MercenaryDefinition>
{
    public override string localTable => LocalTable.MercenaryTable;

    public List<SkillData> skillDataList = new();

    public float dashSpeed => 0.2f;
    public int dashCount => _data.dashCount;
    public float dashCooldown => _data.dashCooldown;
    public Sprite icon => _data.icon;

    public bool isOwned => saveData?.isOwned ?? false;

    private MercenarySaveData saveData;

    public Mercenary(MercenaryDefinition data) : base(data)
    {
        foreach (var skillDefinition in _data.skillTreeDefinition.skillDefinitions)
        {
            skillDataList.Add(new SkillData(skillDefinition));
        }

        skillDataList.Last().lastData = true;
    }

    public void ApplySaveData(MercenarySaveData saveData)
    {
        this.saveData = new MercenarySaveData(saveData);
    }

    public MercenarySaveData GetSaveData()
    {
        return saveData;
    }

    public bool Acquire()
    {
        if (saveData == null || isOwned == true)
        {
            return false;
        }

        saveData.isOwned = true;
        return true;
    }
}