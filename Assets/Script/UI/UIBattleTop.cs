using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleTop : MonoBehaviour
{
    [SerializeField]
    private Button _btnPause;

    [SerializeField]
    private TextMeshProUGUI _txtGold;
    [SerializeField]
    private TextMeshProUGUI _txtElementWater;
    [SerializeField]
    private TextMeshProUGUI _txtElementForest;
    [SerializeField]
    private TextMeshProUGUI _txtElementFire;

    private void Awake()
    {
        _txtGold.text = "0";

        _btnPause.onClick.AddListener(OnClickPause);

        EventHelper.AddEventListener(EventName.AddGold, OnAddGold);
        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.AddGold, OnAddGold);
        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
    }

    private void OnClickPause()
    {
        BattleManager.instance.SetBattleStatus(BattleStatus.Pause);

        PopupManager.ShowPopup<UIPause>(PopupName.UIPause);
    }


    private void OnAddGold(object sender, object data)
    {
        _txtGold.text = (string)data;
    }
    private void OnAddElement(object sender, object data)
    {
        if (data is Dictionary<ElementType, int> elements == false)
        {
            return;
        }

        foreach (var element in elements)
        {
            switch (element.Key)
            {
                case ElementType.Water:
                    _txtElementWater.text = element.Value.ToString();
                    break;

                case ElementType.Forest:
                    _txtElementForest.text = element.Value.ToString();
                    break;

                case ElementType.Fire:
                    _txtElementFire.text = element.Value.ToString();
                    break;
            }
        }
    }
}
