using TMPro;
using UnityEngine;


public class UIStatItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtStat;

    public void SetStat(StatType type, float value)
    {
        _txtStat.text = $"{type.ToName()} : {value.ToString()}";
    }
}
