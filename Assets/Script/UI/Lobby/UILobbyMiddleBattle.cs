using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyMiddleBattle : UILobbyMiddle
{
    [SerializeField]
    private VerticalLayoutGroup _contentGorup;

    private List<UISVPopupContent> _contents = new List<UISVPopupContent>();

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

        var mapContent = content as UISelectMapContent;
        SceneLoader.LoadBattleScene(mapContent.data);
    }
}
