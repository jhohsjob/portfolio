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

    private MercenaryPVO _pvo;

    public override int level => _pvo != null ? _pvo.level : 1;
    public override int grade => _pvo != null ? _pvo.grade : 0;
    public int availableMaxLevel;
    public bool isOwned => _pvo?.isOwned ?? false;

    public Mercenary(MercenaryDefinition data) : base(data)
    {
        foreach (var skillDefinition in _data.skillTreeDefinition.skillDefinitions)
        {
            skillDataList.Add(new SkillData(skillDefinition));
        }

        skillDataList.Last().lastData = true;
    }

    public void ApplyPVO(MercenaryPVO pvo)
    {
        _pvo = new MercenaryPVO(pvo);

        _stat.SetLevel(_pvo.level);
    }
}