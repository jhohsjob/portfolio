using TMPro;
using UnityEngine;


public class HPBar : MonoBehaviour
{
    private TextMeshProUGUI _hp = null;
    
    private ActorBase _actor = null;
    private Camera _uiCamera;
    private RectTransform _parentRT;

    private Vector2 _offset = new Vector3(0f, 100f);

    private void Awake()
    {
        _hp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_actor == null)
        {
            return;
        }

        var screenPos = Camera.main.WorldToScreenPoint(_actor.transform.position);
        screenPos.z = 0f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRT, screenPos, _uiCamera, out var localPos);

        transform.localPosition = localPos + _offset;

    }

    public void Init(Camera uiCamera, RectTransform parentRT)
    {
        _uiCamera = uiCamera;
        _parentRT = parentRT;

        gameObject.SetActive(false);
    }

    public void SetTarget(ActorBase actor)
    {
        _actor = actor;
        _actor.cbChangeHP += OnChangeHP;

        _hp.text = actor.HP + " / " + actor.maxHP;
        gameObject.SetActive(true);
    }

    public void ResetTarget()
    {
        _actor.cbChangeHP -= OnChangeHP;
        _actor = null;

        gameObject.SetActive(false);
    }

    private void OnChangeHP(ChangeHPData data)
    {
        _hp.text = data.remainHP + " / " + _actor.maxHP;
    }
}
