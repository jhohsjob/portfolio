using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleStageScrollItemData
{
    public Stage stage;
    public StageService.StageState state;
}

public class UIBattleStageScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _imgLock;

    private Stage _data;
    private Action<Stage> _onClick;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        if (data is not UIBattleStageScrollItemData itemData)
        {
            return;
        }
        _data = itemData.stage;

        _name.text = _data.name;

        var state = itemData.state;
        switch (state)
        {
            case StageService.StageState.Lock:
                _imgLock.gameObject.SetActive(true);
                _btn.enabled = false;
                break;

            case StageService.StageState.Current:
                _imgLock.gameObject.SetActive(false);
                _btn.enabled = true;
                break;

            case StageService.StageState.Clear:
                _imgLock.gameObject.SetActive(false);
                _btn.enabled = false;
                break;
        }
    }

    public void SetOnClick(Action<Stage> onClick)
    {
        _onClick = onClick;
    }

    public void OnClickItem()
    {
        _onClick?.Invoke(_data);
    }
}