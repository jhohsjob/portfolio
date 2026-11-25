#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;


public class MercenaryStorage : JsonListStorage<MercenaryPVO>
{
    private const string FileName = "mercenary_save.json";

    private Dictionary<int, MercenaryDefinition> _mercenaryDefinitions;

    public IReadOnlyList<MercenaryPVO> pvos => items;

    public MercenaryStorage() : base(FileName) { }

    public void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries)
    {
        _mercenaryDefinitions = new Dictionary<int, MercenaryDefinition>(mercenaries);
    }

    protected override int GetId(MercenaryPVO item) => item.id;

    protected override List<MercenaryPVO> CreateDefault()
    {
        if (_mercenaryDefinitions == null)
        {
            throw new InvalidOperationException("SetupMercenaries must be called before LoadAsync");
        }

        var list = new List<MercenaryPVO>();
        foreach (var definition in _mercenaryDefinitions.Values)
        {
            list.Add(new MercenaryPVO
            {
                id = definition.Id,
                isOwned = definition.acquire.type == MercenaryAcquireType.Default,
                level = 1,
                grade = 0,
                useLevelUpGold = 0
            });
        }

        return list;
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_mercenary")]
    public static void Delete() => StoragePaths.Delete(FileName);
#endif
}
