using System;
using UnityEngine;
using UnityEngine.UI;


public class UITab : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private GameObject _selectedObject;

    private Action _onClick;

    private void Awake()
    {
        _selectedObject.SetActive(false);
    }

    public void Initialize(Action onClick)
    {
        _onClick = onClick;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }

    public void SetSelected(bool selected)
    {
        _selectedObject.SetActive(selected);
    }

    private void OnClick()
    {
        _onClick?.Invoke();
    }
}