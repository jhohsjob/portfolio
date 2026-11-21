using System.Collections.Generic;
using System.Threading.Tasks;


public class MockGachaDomain
{
    private readonly GachaStorage _gachaStorage;

    private Dictionary<int, GachaDefinition> _gachas = new();

    public MockGachaDomain()
    {
        _gachaStorage = new();
    }

    public void SetupGacha(Dictionary<int, GachaDefinition> gachas)
    {
        _gachas = new Dictionary<int, GachaDefinition>(gachas);

        _gachaStorage.SetupGacha(gachas);
    }

    public async Task<List<GachaPVO>> InitializeAsync()
    {
        await _gachaStorage.LoadAsync();
        return new List<GachaPVO>(_gachaStorage.pvos);
    }

    public GachaDefinition GetGachaDefinitionById(int id)
    {
        _gachas.TryGetValue(id, out var definition);
        return definition;
    }

    public GachaPVO GetGachaPVOById(int id)
    {
        return _gachaStorage.GetById(id);
    }
}