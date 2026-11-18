using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


public interface IDeviceSettings
{
    float bgmVolume { get; }
    float sfxVolume { get; }
    bool mute { get; }

    void SetBgmVolume(float volume);
    void SetSfxVolume(float volume);
    void SetMute(bool mute);

    void Apply();
    void Flush();
}

public sealed class DeviceSettings : IDeviceSettings
{
    private const int SaveDelayMs = 500;

    private readonly Storage _storage;
    private readonly SaveService _saveService;
    private readonly ISoundService _soundService;
    private readonly LocaleService _localeService;

    private CancellationTokenSource _saveCts;
    private bool _localeBound;

    public float bgmVolume => _storage.data != null ? _storage.data.bgmVolume : 1f;
    public float sfxVolume => _storage.data != null ? _storage.data.sfxVolume : 1f;
    public bool mute => _storage.data != null && _storage.data.mute;

    public DeviceSettings(Storage storage, SaveService saveService, ISoundService soundService, LocaleService localeService)
    {
        _storage = storage;
        _saveService = saveService;
        _soundService = soundService;
        _localeService = localeService;
    }

    // Storage.LoadAsync 이후 호출: 저장된 값을 서비스에 반영
    public void Apply()
    {
        var data = _storage.data;
        if (data == null)
        {
            Debug.LogWarning("DeviceSettings.Apply called before Storage was loaded");
            return;
        }

        _soundService.SetBGMVolume(data.bgmVolume);
        _soundService.SetSFXVolume(data.sfxVolume);
        _soundService.SetMute(data.mute);

        _localeService.Apply(data.locale);

        if (_localeBound == false)
        {
            _localeService.changed += HandleLocaleChanged;
            _localeBound = true;
        }
    }

    public void SetBgmVolume(float volume)
    {
        if (TryGetData(out var data) == false)
        {
            return;
        }

        data.bgmVolume = Mathf.Clamp01(volume);
        _soundService.SetBGMVolume(data.bgmVolume);
        ScheduleSave();
    }

    public void SetSfxVolume(float volume)
    {
        if (TryGetData(out var data) == false)
        {
            return;
        }

        data.sfxVolume = Mathf.Clamp01(volume);
        _soundService.SetSFXVolume(data.sfxVolume);
        ScheduleSave();
    }

    public void SetMute(bool mute)
    {
        if (TryGetData(out var data) == false)
        {
            return;
        }

        data.mute = mute;
        _soundService.SetMute(mute);
        ScheduleSave();
    }

    public void Flush()
    {
        if (_saveCts == null)
        {
            return;
        }

        CancelPendingSave();
        _saveService.RequestSave();
    }

    private void HandleLocaleChanged(string code)
    {
        if (TryGetData(out var data) == false)
        {
            return;
        }
        if (data.locale == code)
        {
            return;
        }

        data.locale = code;
        ScheduleSave();
    }

    private bool TryGetData(out GameSaveData data)
    {
        data = _storage.data;
        if (data == null)
        {
            Debug.LogWarning("DeviceSettings changed before Storage was loaded");
            return false;
        }
        return true;
    }

    private void ScheduleSave()
    {
        CancelPendingSave();
        _saveCts = new CancellationTokenSource();
        DelayedSaveAsync(_saveCts.Token).Forget();
    }

    private async UniTaskVoid DelayedSaveAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(SaveDelayMs, ignoreTimeScale: true, cancellationToken: ct);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        _saveCts.Dispose();
        _saveCts = null;
        _saveService.RequestSave();
    }

    private void CancelPendingSave()
    {
        if (_saveCts == null)
        {
            return;
        }

        _saveCts.Cancel();
        _saveCts.Dispose();
        _saveCts = null;
    }
}