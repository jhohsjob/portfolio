using System.Threading.Tasks;


public class MockMercenaryHandler
{
    private readonly MockMercenaryService _mercenaryService;
    private readonly MockUserService _userService;

    public MockMercenaryHandler(MockMercenaryService mercenary, MockUserService user)
    {
        _mercenaryService = mercenary;
        _userService = user;
    }

    public async Task<MercenaryAcquireResponse> MercenaryAcquireAsync(MercenaryAcquireRequest request)
    {
        MercenaryAcquireResponse response = new();

        var definition = _mercenaryService.GetMercenaryDefinitionById(request.mercenaryId);
        if (definition == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED_ID;
            return response;
        }

        var pvo = _mercenaryService.GetMercenaryPVOById(request.mercenaryId);
        if (pvo == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED;
            return response;
        }

        if (pvo.isOwned == true)
        {
            response.result = NetworkResult.E_MERCENARY_ALREADY_OWNED;
            return response;
        }

        if (definition.acquire.type == MercenaryAcquireType.CurrencyGold)
        {
            var gold = _userService.gold;
            if (gold < definition.acquire.value)
            {
                response.result = NetworkResult.E_MERCENARY_NOT_ENOUGH_GOLD;
                return response;
            }

            await _mercenaryService.AcquireAsync(request.mercenaryId);

            await _userService.ChangeGold(-definition.acquire.value);

            response.mercenary = _mercenaryService.GetMercenaryPVOById(request.mercenaryId);
            response.user = _userService.GetUser();
        }

        return response;
    }

    public async Task<MercenaryLevelUpResponse> MercenaryLevelUpAsync(MercenaryLevelUpRequest request)
    {
        MercenaryLevelUpResponse response = new();

        var definition = _mercenaryService.GetMercenaryDefinitionById(request.mercenaryId);
        if (definition == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED_ID;
            return response;
        }

        var pvo = _mercenaryService.GetMercenaryPVOById(request.mercenaryId);
        if (pvo == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED;
            return response;
        }

        if (pvo.isOwned == false)
        {
            response.result = NetworkResult.E_MERCENARY_NOT_OWNED;
            return response;
        }

        var cost = _mercenaryService.GetLevelUpCost(pvo.level, request.targetLevel);

        var checkResult = _mercenaryService.CheckLevelUp(pvo.level, request.targetLevel, _userService.level, _userService.gold);
        if (checkResult != MercenaryLevelUpResult.Success)
        {
            switch (checkResult)
            {
                case MercenaryLevelUpResult.MaxLevel:
                    response.result = NetworkResult.E_MERCENARY_INVALIED_ID;
                    break;
                case MercenaryLevelUpResult.InvalidLevel:
                    response.result = NetworkResult.E_MERCENARY_INVALIED_LEVEL;
                    break;
                case MercenaryLevelUpResult.NotEnoughGold:
                    response.result = NetworkResult.E_MERCENARY_NOT_ENOUGH_GOLD;
                    break;
            }

            return response;
        }

        await _mercenaryService.LevelUpAsync(request.mercenaryId, request.targetLevel, cost);

        await _userService.ChangeGold(-cost);

        response.mercenary = pvo;
        response.user = _userService.GetUser();

        return response;
    }

    public async Task<MercenaryLevelResetResponse> MercenaryLevelResetAsync(MercenaryLevelResetRequest request)
    {
        MercenaryLevelResetResponse response = new();

        var definition = _mercenaryService.GetMercenaryDefinitionById(request.mercenaryId);
        if (definition == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED_ID;
            return response;
        }

        var pvo = _mercenaryService.GetMercenaryPVOById(request.mercenaryId);
        if (pvo == null)
        {
            response.result = NetworkResult.E_MERCENARY_INVALIED;
            return response;
        }

        if (pvo.isOwned == false)
        {
            response.result = NetworkResult.E_MERCENARY_NOT_OWNED;
            return response;
        }

        await _userService.ChangeGold(pvo.useGold);

        await _mercenaryService.LevelResetAsync(request.mercenaryId);

        response.mercenary = pvo;
        response.user = _userService.GetUser();

        return response;
    }
}