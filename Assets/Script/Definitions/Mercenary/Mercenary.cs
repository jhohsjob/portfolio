using UnityEngine;


public class Mercenary : Role<MercenaryData>
{
    public SkillData skillData => _data.skillData;

    public float dashSpeed => 0.2f;
    public int dashCount => _data.dashCount;
    public float dashCooldown => _data.dashCooldown;
    public Sprite icon => _data.icon;

    public bool isOwned => saveData?.isOwned ?? false;

    private MercenarySaveData saveData;

    public Mercenary(MercenaryData data) : base(data) { }

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