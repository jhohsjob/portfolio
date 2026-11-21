using UnityEngine;


public static class AppRoot
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