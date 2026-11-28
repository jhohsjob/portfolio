using TMPro;
using UnityEngine;


public class UIGold : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtGold;

    public void SetGoldText(int amount)
    {
        _txtGold.text = amount.ToString();
    }
}
