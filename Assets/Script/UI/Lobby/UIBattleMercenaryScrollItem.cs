using UnityEngine;
using UnityEngine.UI;


public class UIBattleMercenaryScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private UIMercIcon _mercIcon;

    private Mercenary _mercenary;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _mercenary = (Mercenary)data;

        _mercIcon.Initialize(_mercenary);
    }

    public void OnClickItem()
    {
        _scroll.MoveToIndex(index);
    }
}