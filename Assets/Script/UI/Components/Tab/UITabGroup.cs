using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UITabElement
{
    public UITab tab;
    public GameObject content;
}

public class UITabGroup : MonoBehaviour
{
    [SerializeField]
    private List<UITabElement> _tabElements;

    private int _selectedIndex = -1;

    private void Awake()
    {
        for (int i = 0; i < _tabElements.Count; i++)
        {
            int index = i;
            _tabElements[i].tab.Initialize(() => Select(index));
            _tabElements[i].content.SetActive(false);
        }
    }

    private void Start()
    {
        Select(0);
    }

    public void Select(int index)
    {
        if (index < 0 || index >= _tabElements.Count)
        {
            return;
        }

        if (_selectedIndex == index)
        {
            return;
        }

        if (_selectedIndex >= 0)
        {
            _tabElements[_selectedIndex].tab.SetSelected(false);
            _tabElements[_selectedIndex].content.SetActive(false);
        }

        _selectedIndex = index;

        _tabElements[_selectedIndex].tab.SetSelected(true);
        _tabElements[_selectedIndex].content.SetActive(true);
    }
}