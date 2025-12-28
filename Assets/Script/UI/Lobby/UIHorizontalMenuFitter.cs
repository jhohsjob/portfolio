using System;
using UnityEngine;
using UnityEngine.UI;


public class UIHorizontalMenuFitter : MonoBehaviour
{
    [SerializeField]
    private int _initialIndex = 0;
    [Range(1f, 3f)]
    public float bigRatio = 1.5f;

    private Button[] buttons;

    private float _totalWidth;

    private float _normalWidth;
    private float _bigWidth;

    private Action<int> _cbInitialized;
    public event Action<int> cbInitialized
    {
        add { _cbInitialized -= value; _cbInitialized += value; }
        remove { _cbInitialized -= value; }
    }

    private Action<int> _cbMenuChange;
    public event Action<int> cbMenuChange
    {
        add { _cbMenuChange -= value; _cbMenuChange += value; }
        remove { _cbMenuChange -= value; }
    }

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnClickButton(index));
        }

        RectTransform parent = GetComponent<RectTransform>();
        _totalWidth = parent.rect.width;

        PreCalculateWidths();
        RefreshLayout(_initialIndex);

        _cbInitialized?.Invoke(buttons.Length);
        _cbMenuChange?.Invoke(_initialIndex);
    }

    private void PreCalculateWidths()
    {
        int count = buttons.Length;

        float denominator = bigRatio + (count - 1);
        _normalWidth = _totalWidth / denominator;

        _bigWidth = _normalWidth * bigRatio;
    }
    
    private void RefreshLayout(int index)
    {
        float x = 0f;

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform child = buttons[i].GetComponent<RectTransform>();

            float width = (i == index) ? _bigWidth : _normalWidth;

            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            float pivotOffset = width * child.pivot.x;
            child.anchoredPosition = new Vector2(x + pivotOffset, child.anchoredPosition.y);

            x += width;
        }
    }

    private void OnClickButton(int index)
    {
        RefreshLayout(index);

        _cbMenuChange?.Invoke(index);
    }

    [ContextMenu("Refresh Layout")]
    private void EditorRefresh()
    {
        buttons = GetComponentsInChildren<Button>();
        var parent = GetComponent<RectTransform>();

        _totalWidth = parent.rect.width;
        PreCalculateWidths();
        RefreshLayout(_initialIndex);
    }
}