using System;
using UnityEngine;
using UnityEngine.UI;


public class UIDash : MonoBehaviour
{
    [SerializeField]
    private Button _btnDash;
    [SerializeField]
    private Image _dashCooldown;

    public event Action onClickDash;

    private void Awake()
    {
        _btnDash.onClick.AddListener(() =>
        {
            onClickDash?.Invoke();
        });
    }

    public void SetCooldown(float fillAmount)
    {
        _dashCooldown.fillAmount = fillAmount;
    }
}
