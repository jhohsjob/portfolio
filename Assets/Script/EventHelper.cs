using System;
using System.Collections.Generic;


public static class EventHelper
{
    private static Dictionary<string, Action<object, object>> _actionTable;

    static EventHelper()
    {
        _actionTable = new Dictionary<string, Action<object, object>>();
    }

    public static void Send(string eventName, object sender = null, object data = null)
    {
        if (_actionTable == null) return;

        if (_actionTable.TryGetValue(eventName, out var callback))
        {
            callback?.Invoke(sender, data);
        }
    }

    public static void AddEventListener(string eventName, Action<object, object> newListener)
    {
        if (_actionTable == null) return;

        var isFirst = !_actionTable.TryGetValue(eventName, out var callback);
        
        callback -= newListener;
        callback += newListener;
        
        if (isFirst == true)
        {
            _actionTable.Add(eventName, callback);
        }
        else
        {
            _actionTable[eventName] = callback;
        }
    }

    public static void RemoveEventListener(string eventName, Action<object, object> newListener)
    {
        if (_actionTable == null) return;

        if (_actionTable.TryGetValue(eventName, out var callback))
        {
            callback -= newListener;
        }
    }
}
