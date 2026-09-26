using System.Collections.Generic;
using System.Threading.Tasks;


public class MockMercenaryService
{
    private readonly MercenaryStorage _mercenaryStorage;

    private Dictionary<int, MercenaryDefinition> _mercenaryDefinitions;
    private MercenaryLevelSystem _levelSystem;

    public MockMercenaryService()
    {
        _mercenaryStorage = new MercenaryStorage();
        _levelSystem = new MercenaryLevelSystem();
    }

    public void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries)
    {
        _mercenaryDefinitions = mercenaries;

        _mercenaryStorage.SetupMercenaries(mercenaries);
    }

    public void SetupMercenaryLevels(List<MercenaryLevelData> mercenaryLevels)
    {
        _levelSystem.Setup(mercenaryLevels);
    }

    public async Task<List<MercenaryPVO>> InitializeAsync()
    {
        await _mercenaryStorage.LoadAsync();
        return new List<MercenaryPVO>(_mercenaryStorage.pvos);
    }

    public async Task AcquireAsync(int id)
    {
        var mercenary = _mercenaryStorage.GetById(id);
        mercenary.isOwned = true;

        await _mercenaryStorage.UpdateAsync(mercenary);
    }

    public async Task LevelUpAsync(int id, int targetLevel, int useGold)
    {
        var mercenary = _mercenaryStorage.GetById(id);
        mercenary.level = targetLevel;
        mercenary.useGold += useGold;

        await _mercenaryStorage.UpdateAsync(mercenary);
    }

    public async Task LevelResetAsync(int id)
    {
        var mercenary = _mercenaryStorage.GetById(id);
        mercenary.level = 1;
        mercenary.useGold = 0;

        await _mercenaryStorage.UpdateAsync(mercenary);
    }

    public Task<List<MercenaryPVO>> GetMercenariesAsync()
    {
        return Task.FromResult(new List<MercenaryPVO>(_mercenaryStorage.pvos));
    }

    public Task<MercenaryPVO> GetMercenaryAsync(int id)
    {
        var mercenary = _mercenaryStorage.GetById(id);

        return Task.FromResult(mercenary);
    }

    // default function
    public MercenaryDefinition GetMercenaryDefinitionById(int id)
    {
        _mercenaryDefinitions.TryGetValue(id, out var definition);
        return definition;
    }

    public MercenaryPVO GetMercenaryPVOById(int id)
    {
        return _mercenaryStorage.GetById(id);
    }

    public MercenaryLevelUpResult CheckLevelUp(int level, int targetLevel, int userLevel, int gold)
    {
        return _levelSystem.CheckLevelUp(level, targetLevel, userLevel, gold);
    }

    public int GetLevelUpCost(int level, int targetLevel)
    {
        return _levelSystem.GetLevelUpCost(level, targetLevel);
    }
}