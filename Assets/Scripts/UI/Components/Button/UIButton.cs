using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private TextMeshProUGUI _text;

    public string text
    {
        get => _text.text;
        set => _text.text = value;
    }

    public bool interactable
    {
        get => _button.interactable;
        set => _button.interactable = value;
    }

    private void Awake()
    {
        _button ??= GetComponent<Button>();
    }

    public void AddListener(UnityAction action)
    {
        _button.onClick.AddListener(action);
    }

    public void RemoveListener(UnityAction action)
    {
        _button?.onClick.RemoveListener(action);
    }
}