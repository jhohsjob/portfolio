using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyMiddleBattle : UILobbyMiddle, IScrollDataProvider
{
    [SerializeField]
    private VerticalLayoutGroup _contentGorup;

    private List<UISVPopupContent> _contents = new List<UISVPopupContent>();

    [SerializeField]
    private HorizontalInfiniteScroll _scroll;

    private List<Mercenary> _mercenaries;
    private IScrollItemFactory _factory;

    protected override void Awake()
    {
        base.Awake();

        Client.asset.LoadAsset<GameObject>("UISelectMapContent", (task) =>
        {
            var mapInfoList = DataManager.GetAllMapInfoData();
            for (int i = 0; i < mapInfoList.Count; i++)
            {
                var contentData = new UISelectMapConetntData();
                contentData.index = i;
                contentData.data = mapInfoList[i];
                contentData.action = OnClickContent;

                var go = Instantiate(task.GetAsset<GameObject>(), _contentGorup.transform);
                var content = go.GetComponent<UISelectMapContent>();
                content.Init(contentData);

                _contents.Add(content);
            }
        });

        _mercenaries = MercenaryHander.instance.list;
        int listIndex = _mercenaries.FindIndex(x => x.id == Client.user.mercenaryId);

        Client.asset.LoadAsset<GameObject>("BattleMercenaryItem", task =>
        {
            var prefab = task.GetAsset<GameObject>();

            _factory = new MercenaryItemFactory(prefab);

            _scroll.Initialize(
                provider: this,
                factory: _factory,
                itemCount: _mercenaries.Count,
                initPos: listIndex
            );
        });
    }

    private void OnClickContent(int index)
    {
        if (_contents.Count <= index)
        {
            return;
        }

        var content = _contents[index];
        if (content is UISelectMapContent == false)
        {
            return;
        }

        int mIndex = _scroll.GetCenteredIndex();
        if (mIndex < 0 || mIndex >= _mercenaries.Count)
        {
            return;
        }

        Client.user.SetMercenary(_mercenaries[mIndex].id);

        var mapContent = content as UISelectMapContent;
        SceneLoader.LoadBattleScene(mapContent.data);
    }

    public int GetItemCount()
    {
        return _mercenaries.Count;
    }

    public void Bind(int index, InfiniteScrollItem item)
    {
        //item.OnRecycle();
        item.SetData(index, _mercenaries[index]);

        float scale = _scroll.CalculateScaleForItem(item);
        item.ForceSetScale(scale);
    }
}
