using System;
using System.Collections.Generic;


public class StageManager : Singleton<StageManager>
{
    public enum StageState { Lock, Current, Clear }

    /// <summary>
    /// key : stage Id
    /// value : stage
    /// </summary>
    private Dictionary<int, Stage> _dic = new();
    private List<Stage> _list = new();
    public IReadOnlyList<Stage> list => _list;
    public int count { get; private set; }

    public void Init(Dictionary<int, StageDefinition> stageDefinitions)
    {
        _dic.Clear();
        _list.Clear();

        foreach (var pair in stageDefinitions)
        {
            if (_dic.TryAdd(pair.Key, new Stage(pair.Value)) == false)
            {
                throw new InvalidOperationException($"Duplicate Stage ID detected: {pair.Key}");
            }
        }
        _list.AddRange(_dic.Values);
        count = _list.Count;
    }

    public Stage GetStageByIndex(int index)
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index];
    }

    public Stage GetStageById(int id)
    {
        if (_dic.ContainsKey(id) == false)
        {
            return null;
        }

        return _dic[id];
    }

    public int GetStageIndexById(int id)
    {
        return _list.FindIndex(x => x.id == id);
    }

    public int NextStageID(int id)
    {
        int index = GetStageIndexById(id);
        if (index < 0)
        {
            return -1;
        }

        int nextIndex = CalcIndex(index + 1);
        Stage nextStage = GetStageByIndex(nextIndex);
        if (nextStage == null)
        {
            return -1;
        }

        return nextStage.id;
    }

    public int CalcIndex(int index)
    {
        if (index < 0)
        {
            index = 0;
        }
        if (index >= _list.Count)
        {
            index = _list.Count - 1;
        }

        return index;
    }

    public StageState IsStageStateByIndex(int index)
    {
        int currentIndex = GetStageIndexById(Client.user.currentStageId);
        int reverseIndex = (count - 1) - currentIndex;

        if (reverseIndex > index)
        {
            return StageState.Lock;
        }
        else if (reverseIndex == index)
        {
            return StageState.Current;
        }
        else
        {
            return StageState.Clear;
        }
    }
}
