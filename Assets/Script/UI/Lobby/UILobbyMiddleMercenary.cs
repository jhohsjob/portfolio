using System.Collections.Generic;
using UnityEngine;


public class MercenaryItemFactory : IScrollItemFactory
{
    private GameObject _prefab;

    public MercenaryItemFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }
}

public class UILobbyMiddleMercenary : UILobbyMiddle, IScrollDataProvider
{
    [SerializeField]
    private VirtualGridScroll _scroll;

    private List<Mercenary> _mercenaries;
    private IScrollItemFactory _factory;

    protected override void Awake()
    {
        base.Awake();

        _mercenaries = MercenaryHander.instance.list;

        Client.asset.LoadAsset<GameObject>("MercenaryItem", task =>
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

    public void Bind(int index, GameObject item)
    {
        item.GetComponent<UIMercenaryScrollItem>().Init(_mercenaries[index]);
    }
}