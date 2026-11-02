using TMPro;
using UnityEngine;


public class UIGem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtGem;

    public void SetGemText(int amount)
    {
        _txtGem.text = amount.ToString();
    }
}
