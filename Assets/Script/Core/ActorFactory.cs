using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class ActorFactory
{
    private readonly Dictionary<Type, Action<GameObject>> _builders = new();

    public ActorFactory()
    {
        var actorTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ActorBase)) && !t.IsAbstract);

        foreach (var type in actorTypes)
        {
            RegisterDefaultBuilder(type);
        }
    }

    private void RegisterDefaultBuilder(Type type)
    {
        _builders[type] = go =>
        {
            // todo
        };
    }

    public ActorBase CreateActor<TData>(Role<TData> role, Transform parent) where TData : RoleData
    {
        var go = new GameObject(role.behaviourType.Name);
        var actor = (ActorBase)go.AddComponent(role.behaviourType);
        
        Type current = role.behaviourType;
        while (current != typeof(ActorBase))
        {
            if (_builders.TryGetValue(current, out var builder))
            {
                builder(go);
                break;
            }
            current = current.BaseType;
        }

        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;

        actor.InitBase(role);

        return actor;
    }
}
