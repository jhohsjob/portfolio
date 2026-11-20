using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class SaveService
{
    private readonly Storage _storage;

    private bool _saving;
    private bool _dirty;

    public SaveService(Storage storage)
    {
        _storage = storage;
    }

    public async void RequestSave()
    {
        _dirty = true;

        if (_saving)
        {
            return;
        }

        RunSaveLoopAsync().Forget();
    }

    private async UniTaskVoid RunSaveLoopAsync()
    {
        _saving = true;

        try
        {
            while (_dirty)
            {
                _dirty = false;

                var data = _storage.data;
                if (data == null)
                {
                    Debug.LogWarning("Save requested before Storage was loaded");
                    break;
                }

                await _storage.SaveAsync(data);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _saving = false;
        }
    }
}