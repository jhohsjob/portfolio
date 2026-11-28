using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public static class StoragePaths
{
    // persistentDataPath는 메인 스레드에서만 호출 가능하므로, await 이전에 경로를 확정해서 사용한다.
    public static string Get(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

    public static void Delete(string fileName)
    {
        string path = Get(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Debug.Log("delete complete");
    }
}

/// <summary>
/// JSON 파일 1개에 대한 원자적 저장/로드를 담당한다. (tmp 쓰기 → Replace/Move)
/// </summary>
public sealed class JsonFileStorage<T> where T : class
{
    private readonly string _fileName;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonFileStorage(string fileName)
    {
        _fileName = fileName;
    }

    public bool Exists() => File.Exists(StoragePaths.Get(_fileName));

    public async Task SaveAsync(T data)
    {
        // 호출 시점의 스냅샷을 직렬화하고, 경로도 메인 스레드에서 확정한다.
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string savePath = StoragePaths.Get(_fileName);
        string tempPath = savePath + ".tmp";

        // 같은 tmp 파일에 동시 쓰기가 일어나지 않도록 직렬화한다.
        await _gate.WaitAsync();
        try
        {
            await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            await using (var writer = new StreamWriter(fs))
            {
                await writer.WriteAsync(json);
            }

            if (File.Exists(savePath))
            {
                File.Replace(tempPath, savePath, null);
            }
            else
            {
                File.Move(tempPath, savePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed ({_fileName}): {e}");
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }

    public T Load()
    {
        string json = File.ReadAllText(StoragePaths.Get(_fileName));
        var data = JsonConvert.DeserializeObject<T>(json);

        if (data == null)
        {
            throw new Exception($"Save file is corrupted: {_fileName}");
        }

        return data;
    }
}
