using System.Collections.Generic;
using System.Threading.Tasks;


/// <summary>
/// id를 가진 PVO 목록 저장소의 공통 로직.
/// 파생 클래스는 id 추출, 기본 데이터 생성, (선택) 로드 후처리만 구현한다.
/// </summary>
public abstract class JsonListStorage<T> where T : class
{
    private readonly JsonFileStorage<List<T>> _file;
    private List<T> _items = new();

    protected IReadOnlyList<T> items => _items;

    protected JsonListStorage(string fileName)
    {
        _file = new JsonFileStorage<List<T>>(fileName);
    }

    protected abstract int GetId(T item);
    protected abstract List<T> CreateDefault();
    protected virtual void OnLoaded(List<T> loaded) { }

    public T GetById(int id)
    {
        foreach (var item in _items)
        {
            if (GetId(item) == id)
            {
                return item;
            }
        }

        return null;
    }

    public async Task LoadAsync()
    {
        if (_file.Exists())
        {
            _items = _file.Load();
            OnLoaded(_items);
        }
        else
        {
            _items = CreateDefault();
            await _file.SaveAsync(_items);
        }
    }

    public Task SaveAsync() => _file.SaveAsync(_items);

    public async Task UpdateAsync(T changeData)
    {
        int id = GetId(changeData);
        int index = _items.FindIndex(x => GetId(x) == id);
        if (index < 0)
        {
            return;
        }

        _items[index] = changeData;

        await SaveAsync();
    }
}
