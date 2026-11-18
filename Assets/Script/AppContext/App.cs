using UnityEngine;


public sealed class AppContext
{
    public IAssetService AssetService { get; }
    public Storage Storage { get; }
    public SaveService SaveService { get; }
    public LocaleService LocaleService { get; }
    public ISoundService SoundService { get; }
    public Transform PopupContainer { get; }

    public readonly IDeviceSettings DeviceSettings;

    public AppContext(GlobalRoot global)
    {
        AssetService = new AssetService();
        Storage = new Storage(AssetService);
        SaveService = new SaveService(Storage);
        LocaleService = new LocaleService();

        SoundService = new SoundService();
        SoundService.Initialize(global.SoundRoot);
        
        PopupContainer = global.PopupCanvas;

        DeviceSettings = new DeviceSettings(Storage, SaveService, SoundService, LocaleService);
    }

    // Storage.LoadAsync 이후 호출. 저장된 기기 설정을 서비스에 반영한다.
    public void ApplyDeviceSettings() => DeviceSettings.Apply();

    // 앱 종료/일시정지 시 대기 중인 설정 저장을 즉시 처리
    public void FlushDeviceSettings() => DeviceSettings.Flush();
}

public static class App
{
    private static AppContext _context;
    
    public static void FlushDeviceSettings() => _context?.FlushDeviceSettings();

    public static AppContext EnsureCreated(GlobalRoot candidate)
    {
        if (_context != null)
        {
            if (candidate != null)
            {
                Object.Destroy(candidate.gameObject);
            }
            return _context;
        }

        Object.DontDestroyOnLoad(candidate.gameObject);
        _context = new AppContext(candidate);
        return _context;
    }
}