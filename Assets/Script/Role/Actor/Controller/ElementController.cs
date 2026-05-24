using System;
using System.Collections.Generic;
using System.Linq;


public class ElementController
{
    private Dictionary<ElementType, int> _elements = new();
    private Dictionary<ElementType, int> _elementLevels = new();
    private readonly int _levelupThreshold = 10;

    public Action<ElementType> onLevelup;

    public void Init()
    {
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
        foreach (var key in _elements.Keys.ToList())
        {
            _elements[key] = 0;
        }

        onLevelup?.Invoke(elementType);
    }
}