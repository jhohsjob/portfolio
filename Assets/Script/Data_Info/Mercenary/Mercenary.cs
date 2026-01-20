using UnityEngine;

public class Mercenary : Role<MercenaryData>
{
    public SkillData skillData => _data.skillData;

    public float dashSpeed => 0.2f;
    public int dashCount => _data.dashCount;
    public float dashCooldown => _data.dashCooldown;
    public Sprite icon => _data.icon;

    public Mercenary(MercenaryData data) : base(data) { }
}
