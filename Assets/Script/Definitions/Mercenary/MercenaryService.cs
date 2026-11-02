using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class MercenaryServiceDependencies
{
    public RoleFactory factory;
    public IGameServer gameServer;
    public User user;
    public ICurrencyService currencyService;
}

public class MercenaryService
{
    private RoleFactory _factory;
    private IGameServer _gameServer;
    private User _user;
    private ICurrencyService _currencyService;

    private MercenaryLevelSystem _levelSystem = new();
    private MercenaryGradeSystem _gradeSystem = new();

    private Dictionary<int, MercenaryDefinition> _mercenaryDefinitions;

    /// <summary>
    /// key : mercenary Id
    /// value : mercenary data
    /// </summary>
    private Dictionary<int, Mercenary> _dic = new();
    private List<Mercenary> _list = new();
    public IReadOnlyList<Mercenary> list => _list;

    public MercenaryService(MercenaryServiceDependencies dependencies)
    {
        _factory = dependencies.factory;
        _gameServer = dependencies.gameServer;
        _user = dependencies.user;
        _currencyService = dependencies.currencyService;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
    }

    private void Unbind()
    {
    }

    public void SetupDefinition(Dictionary<int, MercenaryDefinition> mercenaryDatas)
    {
        _mercenaryDefinitions = mercenaryDatas;

        foreach (var data in mercenaryDatas)
        {
            var mercenary = (Mercenary)_factory.Create(data.Value);
            _dic.Add(data.Key, mercenary);
        }
        _list.AddRange(_dic.Values);
    }

    public void SetupLevels(List<MercenaryLevelData> mercenaryLevels)
    {
        _levelSystem.Setup(mercenaryLevels);
    }

    public void SetupGrades(List<MercenaryGradeData> mercenaryGrades)
    {
        _gradeSystem.Setup(mercenaryGrades);
    }

    public void InitializeServer(List<MercenaryPVO> pvos)
    {
        foreach (var pvo in pvos)
        {
            ApplyPVO(pvo);
        }
    }

    public void ApplyPVOs(Dictionary<int, MercenaryPVO> pvos)
    {
        foreach (var mercenary in _dic.Values)
        {
            if (pvos.TryGetValue(mercenary.id, out var pvo))
            {
                ApplyPVO(pvo);
            }
        }
    }

    public void ApplyPVO(MercenaryPVO pvo)
    {
        if (_dic.TryGetValue(pvo.id, out var mercenary))
        {
            mercenary.ApplyPVO(pvo);
        }
        else
        {
            Debug.LogWarning($"Mercenary with ID {pvo.id} not found in the dictionary.");
        }
    }

    public MercenaryDefinition GetMercDefinitionById(int id)
    {
        _mercenaryDefinitions.TryGetValue(id, out var mercenary);
        return mercenary;
    }

    public Mercenary GetMercenaryByIndex(int index)
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index];
    }

    public Mercenary GetMercenaryById(int id)
    {
        _dic.TryGetValue(id, out var mercenary);
        return mercenary;
    }

    public Mercenary GetNext(Mercenary mercenary)
    {
        if (mercenary == null)
        {
            return null;
        }

        int index = _list.IndexOf(mercenary);
        if (index < 0)
        {
            return null;
        }

        index++;
        index = CalcIndex(index);
        return _list[index];
    }

    public Mercenary GetPrev(Mercenary mercenary)
    {
        if (mercenary == null)
        {
            return null;
        }
        int index = _list.IndexOf(mercenary);
        if (index < 0)
        {
            return null;
        }
        index--;
        index = CalcIndex(index);
        return _list[index];
    }

    public bool Has(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary))
        {
            return mercenary.isOwned;
        }
        return false;
    }

    public int GetAvailableMaxLevel(int level, int userLevel, int gold)
    {
        return _levelSystem.GetAvailableMaxLevel(level, userLevel, gold);
    }

    public async Task<MercenaryLevelUpResult> LevelUp(int id, int targetLevel)
    {
        if (_dic.TryGetValue(id, out var mercenary) == false)
        {
            return MercenaryLevelUpResult.InvalidId;
        }

        if (mercenary.isOwned == false)
        {
            return MercenaryLevelUpResult.NotOwned;
        }

        var check = _levelSystem.CheckLevelUp(mercenary.level, targetLevel, _user.level, _currencyService.Get(CurrencyType.Gold));
        if (check != MercenaryLevelUpResult.Success)
        {
            return check;
        }

        MercenaryLevelUpRequest request = new MercenaryLevelUpRequest
        {
            mercenaryId = id,
            targetLevel = targetLevel
        };

        var response = await _gameServer.MercenaryLevelUpAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            _user.ApplyPVO(response.user);

            return MercenaryLevelUpResult.Success;
        }

        return response.result switch
        {
            NetworkResult.E_MERCENARY_INVALID => MercenaryLevelUpResult.Invalid,
            NetworkResult.E_MERCENARY_INVALID_ID => MercenaryLevelUpResult.InvalidId,
            NetworkResult.E_MERCENARY_NOT_OWNED => MercenaryLevelUpResult.NotOwned,
            NetworkResult.E_MERCENARY_MAX_LEVEL => MercenaryLevelUpResult.MaxLevel,
            NetworkResult.E_MERCENARY_INVALID_LEVEL => MercenaryLevelUpResult.InvalidLevel,
            NetworkResult.E_MERCENARY_NOT_ENOUGH_GOLD => MercenaryLevelUpResult.NotEnoughGold,
            _ => MercenaryLevelUpResult.Invalid
        };
    }

    public async Task<MercenaryLevelResetResult> LevelReset(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary) == false)
        {
            return MercenaryLevelResetResult.InvalidId;
        }

        if (mercenary.isOwned == false)
        {
            return MercenaryLevelResetResult.NotOwned;
        }

        MercenaryLevelResetRequest request = new MercenaryLevelResetRequest
        {
            mercenaryId = id
        };

        var response = await _gameServer.MercenaryLevelResetAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            _user.ApplyPVO(response.user);

            return MercenaryLevelResetResult.Success;
        }

        return response.result switch
        {
            NetworkResult.E_MERCENARY_INVALID => MercenaryLevelResetResult.Invalid,
            NetworkResult.E_MERCENARY_INVALID_ID => MercenaryLevelResetResult.InvalidId,
            NetworkResult.E_MERCENARY_NOT_OWNED => MercenaryLevelResetResult.NotOwned,
            _ => MercenaryLevelResetResult.Invalid
        };
    }

    public bool CanGradeUp(int grade, int targetGrade, int shardCount)
    {
        return _gradeSystem.CheckGradeUp(grade, targetGrade, shardCount) == MercenaryGradeUpResult.Success;
    }

    public int GetGradeUpCost(int grade, int targetGrade)
    {
        return _gradeSystem.GetGradeUpCost(grade, targetGrade);
    }

    public async Task<MercenaryGradeUpResult> GradeUp(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary) == false)
        {
            return MercenaryGradeUpResult.InvalidId;
        }

        if (mercenary.isOwned == false)
        {
            return MercenaryGradeUpResult.NotOwned;
        }

        MercenaryGradeUpRequest request = new MercenaryGradeUpRequest
        {
            mercenaryId = id,
            targetGrade = mercenary.grade + 1
        };

        var response = await _gameServer.MercenaryGradeUpAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            return MercenaryGradeUpResult.Success;
        }

        return response.result switch
        {
            NetworkResult.E_MERCENARY_INVALID => MercenaryGradeUpResult.Invalid,
            NetworkResult.E_MERCENARY_INVALID_ID => MercenaryGradeUpResult.InvalidId,
            NetworkResult.E_MERCENARY_NOT_OWNED => MercenaryGradeUpResult.NotOwned,
            NetworkResult.E_MERCENARY_MAX_GRADE => MercenaryGradeUpResult.MaxGrade,
            NetworkResult.E_MERCENARY_INVALID_GRADE => MercenaryGradeUpResult.InvalidGrade,
            NetworkResult.E_MERCENARY_NOT_ENOUGH_SHARD => MercenaryGradeUpResult.NotEnoughShard,
            _ => MercenaryGradeUpResult.Invalid
        };
    }

    public int CalcIndex(int index)
    {
        if (index < 0)
        {
            index = _list.Count - 1;
        }
        if (index >= _list.Count)
        {
            index = 0;
        }

        return index;
    }
}
