#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Threading.Tasks;


public class UserStorage
{
    private const string FileName = "user_save.json";

    private readonly IAssetService _assetService;
    private readonly JsonFileStorage<UserPVO> _file = new(FileName);

    private UserPVO _pvo = new();
    public UserPVO pvo => _pvo;

    public UserStorage(IAssetService assetService)
    {
        _assetService = assetService;
    }

    public async Task LoadAsync()
    {
        if (_file.Exists())
        {
            _pvo = _file.Load();
        }
        else
        {
            _pvo = await LoadDefaultDataAsync();
            await _file.SaveAsync(_pvo);
        }
    }

    public async Task SaveAsync(UserPVO saveData)
    {
        _pvo = saveData;
        await _file.SaveAsync(_pvo);
    }

    public Task Update(UserPVO changeData) => SaveAsync(changeData);

    private Task<UserPVO> LoadDefaultDataAsync()
    {
        var tcs = new TaskCompletionSource<UserPVO>();

        _assetService.LoadData("UserDefaultData", data =>
        {
            if (data is not UserDefaultData userData)
            {
                tcs.TrySetException(new Exception("UserDefaultData load failed"));
                return;
            }

            tcs.TrySetResult(new UserPVO
            {
                id = 1001,
                name = "Default User",
                level = userData.level,
                exp = userData.exp,
                gold = userData.gold,
                gem = userData.gem,
                lastPlayMercenaryId = userData.mercenaryId,
                lastPlayStageId = userData.stageId,
            });
        },
        () => tcs.TrySetException(new Exception("UserDefaultData load failed")));

        return tcs.Task;
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_user")]
    public static void Delete() => StoragePaths.Delete(FileName);
#endif
}
