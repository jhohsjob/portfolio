using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIMercenaryDetailPopupSkillInfo : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private Button _btnHide;
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtDescription;

    private float _deltaX = 50f;
    private float _deltaY => -_rectTransform.rect.height;

    private void Awake()
    {
        _btnHide.onClick.AddListener(OnClickHide);
    }

    public void Show(Transform itemTransform)
    {
        gameObject.SetActive(true);

        CaclPos(itemTransform);
    }

    public void Hide()
    {
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void SetData(SkillData skillData)
    {
        _txtName.text = skillData.name;
        _txtDescription.text = skillData.description;
    }

    private void CaclPos(Transform itemTransform)
    {
        var pos = itemTransform.position;

        pos.x += _deltaX;
        pos.y += _deltaY;

        transform.position = pos;

        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);

        float left = corners[0].x;
        float right = corners[2].x;

        if (left < 0f)
        {
            pos.x -= left;
        }
        else if (right > Screen.width)
        {
            pos.x -= right - Screen.width;
        }

        transform.position = pos;
    }

    private void OnClickHide()
    {
        Hide();
    }
}