#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;


public class GachaStorage : JsonListStorage<GachaPVO>
{
    private const string FileName = "gacha_save.json";

    private Dictionary<int, GachaDefinition> _gachaDefinitions;

    public IReadOnlyList<GachaPVO> pvos => items;

    public GachaStorage() : base(FileName) { }

    public void SetupGacha(Dictionary<int, GachaDefinition> gachas)
    {
        _gachaDefinitions = gachas;
    }

    protected override int GetId(GachaPVO item) => item.id;

    protected override List<GachaPVO> CreateDefault()
    {
        if (_gachaDefinitions == null)
        {
            throw new InvalidOperationException("SetupGacha must be called before LoadAsync");
        }

        var list = new List<GachaPVO>();
        foreach (var gacha in _gachaDefinitions.Values)
        {
            list.Add(new GachaPVO { id = gacha.Id, gachaCount = 0 });
        }

        return list;
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_gacha")]
    public static void Delete() => StoragePaths.Delete(FileName);
#endif
}
