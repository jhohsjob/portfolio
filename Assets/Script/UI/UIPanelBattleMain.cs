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
    private Button _btnDash;
    [SerializeField]
    private Image _dashCooldown;

    private Player _player;

    protected override void Awake()
    {
        _btnDash.onClick.AddListener(OnButtonDash);

        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);
    }

    protected override void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
    }

    private void Update()
    {
        if (BattleManager.instance.IsBattleRun() == false)
        {
            return;
        }

        if (_player == null)
        {
            _player = BattleManager.instance.battleScene.player;
        }

        float ratio = (_player?.dash?.isOnCooldown ?? false) ? (_player?.dash?.cooldownTimer ?? 0f) / (_player?.dash?.cooldownDuration ?? 1f) : 0f;

        _dashCooldown.fillAmount = ratio;
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

    private void OnButtonDash()
    {
        if (_player?.dash.canDash == false)
        {
            return;
        }

        EventHelper.Send(EventName.ClickBtnDash);
    }
}
