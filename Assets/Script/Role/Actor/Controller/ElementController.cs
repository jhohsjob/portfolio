using System;
using System.Collections.Generic;


public class ElementController
{
    private Player _player;

    private Dictionary<ElementType, int> _elements = new();
    private Dictionary<ElementType, int> _elementLevels = new();
    private readonly int _levelupThreshold = 1;

    protected Action<ElementType> _cbLevelup;
    public event Action<ElementType> cbLevelup
    {
        add { _cbLevelup -= value; _cbLevelup += value; }
        remove { _cbLevelup -= value; }
    }

    public void Init(Player player)
    {
        _player = player;

        _elements.Clear();
    }

    public void AddElement(ActorDIElement element)
    {
        if (element == null)
        {
            return;
        }

        if (_elements.TryGetValue(element.elementType, out int count))
        {
            _elements[element.elementType] = count + 1;
        }
        else
        {
            _elements[element.elementType] = 1;
        }

        if (CheckLevelUp(out ElementType leveledElement))
        {
            LevelUp(leveledElement);
            return;
        }

        EventHelper.Send(EventName.AddElement, this, _elements);
    }

    private bool CheckLevelUp(out ElementType leveledElement)
    {
        foreach (var (type, count) in _elements)
        {
            if (count >= _levelupThreshold)
            {
                leveledElement = type;
                return true;
            }
        }

        leveledElement = ElementType.None;
        return false;
    }

    private void LevelUp(ElementType elementType)
    {
        _elements.Clear();

        _cbLevelup?.Invoke(elementType);
    }
}