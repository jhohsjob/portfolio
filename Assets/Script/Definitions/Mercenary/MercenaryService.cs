using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class MercenaryService
{
    private RoleFactory _factory;
    private User _user;
    private IGameServer _gameServer;

    private MercenaryLevelSystem _levelSystem;

    /// <summary>
    /// key : mercenary Id
    /// value : mercenary data
    /// </summary>
    private Dictionary<int, Mercenary> _dic = new();
    private List<Mercenary> _list = new();
    public IReadOnlyList<Mercenary> list => _list;

    public MercenaryService(RoleFactory factory, User user, IGameServer gameServer)
    {
        _factory = factory;
        _user = user;
        _gameServer = gameServer;

        _levelSystem = new MercenaryLevelSystem();
        
        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        EventHelper.AddEventListener(EventName.UpdateUserPvo, OnUpdateUserPvo);
    }

    private void Unbind()
    {
        EventHelper.AddEventListener(EventName.UpdateUserPvo, OnUpdateUserPvo);
    }

    public void SetupDefinition(Dictionary<int, MercenaryDefinition> mercenaryDatas)
    {
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

    public async Task<MercenaryAcquireResult> Acquire(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary) == false)
        {
            return MercenaryAcquireResult.InvalidId;
        }

        if (mercenary.isOwned == true)
        {
            return MercenaryAcquireResult.AlreadyOwned;
        }

        MercenaryAcquireResult result = MercenaryAcquireResult.Success;

        MercenaryAcquireRequest request = new();
        request.mercenaryId = id;

        var response = await _gameServer.MercenaryAcquireAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            _user.ApplyPVO(response.user);
        }
        else
        {
            switch (response.result)
            {
                case NetworkResult.E_MERCENARY_INVALIED:
                    result = MercenaryAcquireResult.Invalid;
                    break;
                case NetworkResult.E_MERCENARY_INVALIED_ID:
                    result = MercenaryAcquireResult.InvalidId;
                    break;
                case NetworkResult.E_MERCENARY_NOT_ENOUGH_GOLD:
                    result = MercenaryAcquireResult.NotEnoughGold;
                    break;
                case NetworkResult.E_MERCENARY_ALREADY_OWNED:
                    result = MercenaryAcquireResult.AlreadyOwned;
                    break;
            }
        }

        return result;
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

        MercenaryLevelUpResult result = MercenaryLevelUpResult.Success;

        result = _levelSystem.CheckLevelUp(mercenary.level, targetLevel, _user.level, _user.gold);
        if (result != MercenaryLevelUpResult.Success)
        {
            return result;
        }

        MercenaryLevelUpRequest request = new();
        request.mercenaryId = id;
        request.targetLevel = targetLevel;

        var response = await _gameServer.MercenaryLevelUpAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            _user.ApplyPVO(response.user);
        }
        else
        {
            switch (response.result)
            {
                case NetworkResult.E_MERCENARY_INVALIED:
                    result = MercenaryLevelUpResult.Invalid;
                    break;
                case NetworkResult.E_MERCENARY_INVALIED_ID:
                    result = MercenaryLevelUpResult.InvalidId;
                    break;
                case NetworkResult.E_MERCENARY_NOT_OWNED:
                    result = MercenaryLevelUpResult.NotOwned;
                    break;
                case NetworkResult.E_MERCENARY_MAX_LEVEL:
                    result = MercenaryLevelUpResult.MaxLevel;
                    break;
                case NetworkResult.E_MERCENARY_INVALIED_LEVEL:
                    result = MercenaryLevelUpResult.InvalidLevel;
                    break;
                case NetworkResult.E_MERCENARY_NOT_ENOUGH_GOLD:
                    result = MercenaryLevelUpResult.NotEnoughGold;
                    break;
            }
        }

        return result;
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

        MercenaryLevelResetResult result = MercenaryLevelResetResult.Success;

        MercenaryLevelResetRequest request = new();
        request.mercenaryId = id;

        var response = await _gameServer.MercenaryLevelResetAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.mercenary);

            _user.ApplyPVO(response.user);
        }
        else
        {
            switch (response.result)
            {
                case NetworkResult.E_MERCENARY_INVALIED:
                    result = MercenaryLevelResetResult.Invalid;
                    break;
                case NetworkResult.E_MERCENARY_INVALIED_ID:
                    result = MercenaryLevelResetResult.InvalidId;
                    break;
                case NetworkResult.E_MERCENARY_NOT_OWNED:
                    result = MercenaryLevelResetResult.NotOwned;
                    break;
            }
        }

        return result;
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

    private void OnUpdateUserPvo(object sender, object data)
    {
        if (sender is not User user)
        {
            return;
        }

        // _levelSystem.UpdateMaxLevel(user.level, user.mercenaryLevelLimit);
    }
}
