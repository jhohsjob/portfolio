using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class GachaServiceDependencies
{
    public IGameServer gameServer;
    public User user;
    public ICurrencyService currencyService;
    public MercenaryService mercenaryService;
}

public class GachaResultData
{
    public GachaResult result;
    public IReadOnlyList<RewardBase> rewards;
}

public class GachaService
{
    private User _user;
    private IGameServer _gameServer;
    private ICurrencyService _currencyService;
    private MercenaryService _mercenaryService;

    /// <summary>
    /// key : mercenary Id
    /// value : mercenary data
    /// </summary>
    private Dictionary<int, Gacha> _dic = new();
    private List<Gacha> _list = new();
    public IReadOnlyList<Gacha> list => _list;
    private List<GachaPVO> _pvos = new();

    public GachaService(GachaServiceDependencies dependencies)
    {
        _gameServer = dependencies.gameServer;
        _user = dependencies.user;
        _currencyService = dependencies.currencyService;
        _mercenaryService = dependencies.mercenaryService;

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

    public void SetupDefinition(Dictionary<int, GachaDefinition> gachaDefinitions)
    {
        _dic.Clear();
        _list.Clear();

        foreach (var pair in gachaDefinitions)
        {
            if (_dic.TryAdd(pair.Key, new Gacha(pair.Value)) == false)
            {
                throw new InvalidOperationException($"Duplicate Gacha ID detected: {pair.Key}");
            }
        }
        _list.AddRange(_dic.Values);
    }

    public void InitializeServer(List<GachaPVO> pvos)
    {
        _pvos.AddRange(pvos);
    }

    public void ApplyPVO(GachaPVO pvo)
    {
        if (_dic.TryGetValue(pvo.id, out var gacha))
        {
            gacha.ApplyPVO(pvo);
        }
        else
        {
            Debug.Log($"Gacha with ID {pvo.id} not found.");
        }
    }

    public async Task<GachaResultData> GachaAsync(int id, int count)
    {
        if (_dic.TryGetValue(id, out var gacha) == false)
        {
            return new GachaResultData { result = GachaResult.InvalidId };
        }

        GachaResult notEnoughResult = gacha.currencyType switch
        {
            CurrencyType.Gold => GachaResult.NotEnoughGold,
            CurrencyType.Gem => GachaResult.NotEnoughGem,
            _ => GachaResult.Invalid
        };

        if (notEnoughResult == GachaResult.Invalid)
        {
            return new GachaResultData { result = GachaResult.Invalid };
        }

        if (_currencyService.HasEnough(gacha.currencyType, gacha.price * count) == false)
        {
            return new GachaResultData { result = notEnoughResult };
        }

        GachaRequest request = new GachaRequest
        {
            gachaId = id,
            drawCount = count
        };

        GachaResultData result = new GachaResultData();
        result.result = GachaResult.Success;

        var response = await _gameServer.GachaAsync(request);
        
        if (response.result == NetworkResult.SUCCESS)
        {
            ApplyPVO(response.gachaPVO);

            _user.ApplyPVO(response.userPVO);
            _mercenaryService.ApplyPVOs(response.mercenaryPVOs);

            return new GachaResultData
            {
                result = GachaResult.Success,
                rewards = response.rewards
            };
        }

        return new GachaResultData
        {
            result = response.result switch
            {
                NetworkResult.E_GACHA_INVALID => GachaResult.Invalid,
                NetworkResult.E_GACHA_INVALID_ID => GachaResult.InvalidId,
                NetworkResult.E_GACHA_INVALID_DRAW_COUNT => GachaResult.InvalidDrawCount,
                NetworkResult.E_GACHA_NOT_ENOUGH_GOLD => GachaResult.NotEnoughGold,
                NetworkResult.E_GACHA_NOT_ENOUGH_GEM => GachaResult.NotEnoughGem,
                _ => GachaResult.Invalid
            }
        };
    }
}