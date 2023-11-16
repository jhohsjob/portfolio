using TMPro;
using UnityEngine;


public class HPBar : MonoBehaviour
{
    private TextMeshProUGUI _hp = null;
    
    private Actor _actor = null;

    private Vector3 _offset;

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

        transform.position = Camera.main.WorldToScreenPoint(_actor.transform.position);
        transform.position += _offset;
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void SetTarget(Actor actor)
    {
        _actor = actor;
        _actor.cbChangeHP += OnChangeHP;

        _offset = new Vector3(0f, 80f, 0f);

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
