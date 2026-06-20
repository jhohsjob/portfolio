using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image _hp = null;
    [SerializeField]
    private TextMeshProUGUI _debug = null;

    private Vector2 _offset = new Vector2(0f, 60f);

    private void Awake()
    {
    }

    public void FollowPosition(Vector2 localPos)
    {
        transform.localPosition = localPos + _offset;
    }

    public void SetHp(ChangeHPData data)
    {
        _hp.fillAmount = data.fillAmount;
        _debug.text = data.text;
    }
}