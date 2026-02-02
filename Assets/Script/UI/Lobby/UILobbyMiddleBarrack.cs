using System.Collections.Generic;
using UnityEngine;


public class UILobbyMiddleBarrack : UILobbyMiddle, IScrollDataProvider
{
    [SerializeField]
    private VerticalInfiniteScroll _scroll;

    private List<Mercenary> _mercenaries;
    private IScrollItemFactory _factory;

    protected override void Awake()
    {
        base.Awake();

        _mercenaries = MercenaryHander.instance.list;

        Client.asset.LoadAsset<GameObject>("BarrackMercenaryItem", task =>
        {
            var prefab = task.GetAsset<GameObject>();

            _factory = new MercenaryItemFactory(prefab);

            _scroll.Initialize(
                provider: this,
                factory: _factory,
                itemCount: _mercenaries.Count
            );
        });
    }

    public int GetItemCount()
    {
        return _mercenaries.Count;
    }

    public void Bind(int index, InfiniteScrollItem item)
    {
        item.SetData(index, _mercenaries[index]);
    }
}