using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIPanelBattleMain : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _txtKill;
    [SerializeField]
    private TextMeshProUGUI _txtElementWater;
    [SerializeField]
    private TextMeshProUGUI _txtElementForest;
    [SerializeField]
    private TextMeshProUGUI _txtElementFire;
    [SerializeField]
    private Button _btnPause;

    protected override void Awake()
    {
        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);

        _btnPause.onClick.AddListener(OnClickPause);
    }
    protected override void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
    }

    private void OnClickPause()
    {
        BattleManager.instance.SetBattleStatus(BattleStatus.Pause);

        UIManager.instance.ShowPopup(PopupName.UIPause);
    }

    private void OnEnemyDieEnd(object sender, object data)
    {
        if (sender is EnemyManager manager == false)
        {
            return;
        }

        _txtKill.text = manager.dieCount.ToString();
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
