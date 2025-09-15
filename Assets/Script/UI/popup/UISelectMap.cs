using UnityEngine;
using UnityEngine.UI;


public class UISelectMap : UISVPopup
{
    [SerializeField]
    private HorizontalLayoutGroup _contentGorup;

    public override void OnPopupReady(object data = null)
    {
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

        /*
        var contentOriginal = Resources.Load<UISelectMapConetnt>("UI/Popup/UISelectMapContent");

        var mapInfoList = DataManager.GetAllMapInfoData();
        for (int i = 0; i < mapInfoList.Count; i++)
        {
            var contentData = new UISelectMapConetntData();
            contentData.index = i;
            contentData.data = mapInfoList[i];
            contentData.action = OnClickContent;

            var content = Instantiate(contentOriginal, _contentGorup.transform);
            content.Init(contentData);

            _contents.Add(content);
        }
        */
    }

    protected override void OnClickContent(int index)
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

        var mapContent = content as UISelectMapContent;
        SceneLoader.LoadBattleScene(mapContent.data);
    }
}
