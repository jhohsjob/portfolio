using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image _hp = null;
    [SerializeField]
    private TextMeshProUGUI _debug = null;
    
    private ActorBase _actor = null;
    private Camera _uiCamera;
    private RectTransform _parentRT;

    private Vector2 _offset = new Vector2(0f, 100f);

    private void Awake()
    {
    }

    private void Update()
    {
        if (_actor == null)
        {
            return;
        }

        var screenPos = Camera.main.WorldToScreenPoint(_actor.transform.position);
        // screenPos.z = 0f;

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
        actor.hp.cbChange += OnChangeHP;

        gameObject.SetActive(true);

        _hp.fillAmount = actor.hp.currentHP / actor.hp.maxHP;
        _debug.text = actor.hp.currentHP + " / " + actor.hp.maxHP;
    }

    public void ResetTarget()
    {
        _actor.hp.cbChange -= OnChangeHP;
        _actor = null;

        gameObject.SetActive(false);
    }

    private void OnChangeHP(ChangeHPData data)
    {
        _hp.fillAmount = data.fillAmount;
        _debug.text = data.text;
    }
}
