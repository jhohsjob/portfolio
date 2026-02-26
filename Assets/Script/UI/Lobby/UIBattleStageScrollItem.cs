using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

        _data = (Stage)data;

        _name.text = _data.name;

        var state = StageManager.instance.IsStageStateByIndex(index);
        switch (state)
        {
            case StageManager.StageState.Lock:
                _imgLock.gameObject.SetActive(true);
                _btn.enabled = false;
                break;

            case StageManager.StageState.Current:
                _imgLock.gameObject.SetActive(false);
                _btn.enabled = true;
                break;

            case StageManager.StageState.Clear:
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