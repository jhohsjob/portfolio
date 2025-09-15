using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UISelectMapConetntData : UISVPopupContentData
{
    public int index;
    public MapInfoData data;
    public UnityAction<int> action;
}

public class UISelectMapContent : UISVPopupContent
{
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private Button _btn;

    [NonSerialized]
    public MapInfoData data;

    public override void Init(UISVPopupContentData data)
    {
        var contentData = data as UISelectMapConetntData;
        if (contentData == null)
        {
            return;
        }

        index = contentData.index;
        this.data = contentData.data;
        _btn.onClick.AddListener(() => contentData.action(index));

        _txtName.text = this.data.mapName;
    }
}
