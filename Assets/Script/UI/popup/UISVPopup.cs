using System.Collections.Generic;
using UnityEngine;


public class UISVPopup : UIPopup
{
    protected List<UISVPopupContent> _contents = new List<UISVPopupContent>();

    protected override void Clear()
    {
        foreach (var content in _contents)
        {
            GameObject.Destroy(content.gameObject);
        }

        _contents.Clear();

        base.Clear();
    }


    protected virtual void OnClickContent(int index)
    {
        
    }
}
