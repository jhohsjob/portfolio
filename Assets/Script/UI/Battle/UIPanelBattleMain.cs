using TMPro;
using UnityEngine;


public class UIPanelBattleMain : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _txtKill;
    
    protected override void Awake()
    {
        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
    }

    protected override void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);
    }

    private void OnEnemyDieEnd(object sender, object data)
    {
        if (sender is EnemySpawner enemySpawner == false)
        {
            return;
        }

        _txtKill.text = enemySpawner.dieCount.ToString();
    }
}