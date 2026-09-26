using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIStat : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private UIStatItem _itemTemplate;

    private List<UIStatItem> _statItems = new();

    private void Awake()
    {
        _itemTemplate.gameObject.SetActive(false);
    }

    public void Refresh(IStat stat)
    {
        HideItems();
        
        var statTypes = stat.GetAllType();

        if (statTypes.Count > _statItems.Count)
        {
            CreateItems(statTypes.Count - _statItems.Count);
        }

        for (int i = 0; i < statTypes.Count; i++)
        {
            var statType = statTypes[i];
            _statItems[i].gameObject.SetActive(true);
            _statItems[i].SetStat(statType, stat.GetStat(statType));
        }
    }

    private void CreateItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var item = Instantiate(_itemTemplate, _scrollRect.content);
            item.gameObject.SetActive(false);
            _statItems.Add(item);
        }
    }

    private void HideItems()
    {
        for (int i = 0; i < _statItems.Count; i++)
        {
            _statItems[i].gameObject.SetActive(false);
        }
    }
}
