using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


public class UISelectMap : UISVPopup
{
    [SerializeField]
    private HorizontalLayoutGroup _contentGorup;

    private void OnDestroy()
    {
        Debug.Log(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
    }

    protected override void Init(object data = null)
    {
        var contentOriginal = Resources.Load<UISelectMapConetnt>("UI/Popup/UISelectMapContent");

        var mapInfoList = GameTable.GetAllMapInfoData();
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
    }

    protected override void OnClickContent(int index)
    {
        if (_contents.Count <= index)
        {
            return;
        }
        
        var content = _contents[index];
        if (content is UISelectMapConetnt == false)
        {
            return;
        }

        var mapContent = content as UISelectMapConetnt;
        EventHelper.Send(EventName.BattleSceneMove, this, mapContent.data);
    }
}
