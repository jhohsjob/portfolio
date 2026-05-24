public class SaveService
{
    private bool _saving;
    private bool _dirty;

    private GameSaveData _saveData;

    private readonly Storage _storage;

    public SaveService(Storage storage)
    {
        _storage = storage;
    }

    public async void RequestSave()
    {
        _dirty = true;
        _saveData = _storage.data;

        if (_saving)
        {
            return;
        }

        _saving = true;

        do
        {
            _dirty = false;

            await _storage.SaveAsync(_saveData);

        } while (_dirty);

        _saving = false;
    }
}