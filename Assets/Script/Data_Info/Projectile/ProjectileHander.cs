using System.Collections.Generic;

public class ProjectileHander : Singleton<ProjectileHander>
{
    private Dictionary<int, Projectile> _dic = new Dictionary<int, Projectile>();
    private List<Projectile> _list = new List<Projectile>();
    public List<Projectile> list => _list;

    public void Init(Dictionary<int, ProjectileData> ProjectileDatas)
    {
        foreach (var data in ProjectileDatas)
        {
            _dic.Add(data.Key, new Projectile(data.Value));
        }
        _list.AddRange(_dic.Values);
    }

    public Projectile GetProjectileByIndex(int index)
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index];
    }

    public Projectile GetProjectileById(int id)
    {
        if (_dic.ContainsKey(id) == false)
        {
            return null;
        }

        return _dic[id];
    }

    public int CaclIndex(int index)
    {
        if (index < 0)
        {
            index = _list.Count - 1;
        }
        if (index >= _list.Count)
        {
            index = 0;
        }

        return index;
    }
}
