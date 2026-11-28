using System;
using System.Collections.Generic;


public class ProjectileCatalog
{
    private RoleFactory _factory;

    private Dictionary<int, Projectile> _dic = new();
    private List<Projectile> _list = new();
    public IReadOnlyList<Projectile> list => _list;

    public ProjectileCatalog(RoleFactory factory)
    {
        _factory = factory;
    }

    public void Setup(Dictionary<int, ProjectileDefinition> projectileDefinitions)
    {
        if (_factory == null)
        {
            throw new InvalidOperationException("ProjectileManager.Init must be called before Setup");
        }

        var dic = new Dictionary<int, Projectile>(projectileDefinitions.Count);
        foreach (var data in projectileDefinitions)
        {
            dic.Add(data.Key, (Projectile)_factory.Create(data.Value));
        }

        _dic = dic;
        _list = new List<Projectile>(dic.Values);
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

    public int CalcIndex(int index)
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
