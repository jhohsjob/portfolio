using UnityEngine;

public class UISVPopupContentData
{

}

public abstract class UISVPopupContent : MonoBehaviour
{
    public int index;

    public abstract void Init(UISVPopupContentData data = null);
}
