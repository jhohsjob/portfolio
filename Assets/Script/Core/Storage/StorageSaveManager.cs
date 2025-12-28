public static class StorageSaveManager
{
    private static bool _saving;
    private static bool _dirty;

    public static async void RequestSave()
    {
        _dirty = true;

        if (_saving)
        {
            return;
        }

        _saving = true;

        while (_dirty)
        {
            _dirty = false;
            await Client.storage.SaveAsync(Client.storage.data);
        }

        _saving = false;
    }
}
