#if DEBUG_MODE
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;


public class UIDebug : MonoBehaviour
{
    [SerializeField]
    private Button _btnConsole;
    [SerializeField]
    private Transform _root;
    [SerializeField]
    private Transform _buttonContainer;

    [SerializeField]
    private Button _btnAddUserExp;
    [SerializeField]
    private Button _btnAddGold;

    private UIDrag _consoleDrag;

    private UIDebugPresenter _presenter;

    private bool _consoleOpen;

    private readonly float _animationDuration = 0.25f;

    private const float _buttonSize = 100f;
    private const float _buttonSpacing = 30f;
    private const float _initialRadius = 150f;
    private const float _radiusStep = 100f;

    public event Action onClickAddUserExp;
    public event Action onClickAddGold;

    private void Awake()
    {
        _presenter = new UIDebugPresenter(this);

        _consoleDrag = _btnConsole.gameObject.AddComponent<UIDrag>();
        _consoleDrag.SetTarget(_root.transform as RectTransform);

        _btnConsole.onClick.AddListener(OnClickConsole);
        _btnAddUserExp.onClick.AddListener(HandleClickAddUserExp);
        _btnAddGold.onClick.AddListener(HandleClickAddGold);
    }

    private void Start()
    {
        _root.transform.position = new Vector3(Screen.width - 100f, Screen.height - 100f, 0f);
    }

    public void Initialize(UIDebugContext context)
    {
        _presenter.initialize(context);
    }

    private void HandleClickAddUserExp()
    {
        onClickAddUserExp?.Invoke();
    }

    private void HandleClickAddGold()
    {
        onClickAddGold?.Invoke();
    }

    private void OnClickConsole()
    {
        if (_consoleDrag.ConsumeWasDragged())
        {
            return;
        }

        if (_consoleOpen)
        {
            CloseConsole();
        }
        else
        {
            OpenConsole();
        }
    }

    private void OpenConsole()
    {
        int count = _buttonContainer.childCount;

        if (count == 0)
        {
            return;
        }

        int currentIndex = 0;
        int groupIndex = 0;

        while (currentIndex < count)
        {
            float radius = GetRadius(groupIndex);
            int maxCount = GetMaxButtonCount(radius);

            int groupCount = Mathf.Min(maxCount, count - currentIndex);

            float angleStep = 360f / groupCount;

            for (int i = 0; i < groupCount; i++)
            {
                float angle = angleStep * i;
                Vector2 position = GetPosition(angle, radius);

                Transform button = _buttonContainer.GetChild(currentIndex);

                button.gameObject.SetActive(true);
                button.localScale = Vector3.zero;
                button.localPosition = Vector3.zero;

                button.DOScale(Vector3.one, _animationDuration)
                    .SetEase(Ease.OutBack);

                button.DOLocalMove(position, _animationDuration)
                    .SetEase(Ease.OutBack);

                currentIndex++;
            }

            groupIndex++;
        }

        _consoleOpen = true;
    }

    private void CloseConsole()
    {
        _consoleOpen = false;

        int count = _buttonContainer.childCount;

        for (int i = 0; i < count; i++)
        {
            var button = _buttonContainer.GetChild(i);

            button.DOKill();

            button.DOScale(Vector3.zero, _animationDuration);

            button.DOLocalMove(Vector3.zero, _animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    button.gameObject.SetActive(false);
                });
        }
    }

    private Vector2 GetPosition(float angle, float radius)
    {
        float radian = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);
    }

    private int GetMaxButtonCount(float radius)
    {
        float distance = _buttonSize + _buttonSpacing;

        if (distance > radius * 2f)
        {
            return 1;
        }

        return Mathf.FloorToInt(Mathf.PI / Mathf.Asin(distance / (2f * radius)));
    }

    private float GetRadius(int groupIndex)
    {
        return _initialRadius + _radiusStep * groupIndex;
    }
}
#endif