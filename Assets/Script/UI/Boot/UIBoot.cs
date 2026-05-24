using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBoot : MonoBehaviour
{
    private const string LOADING = "Loading";
    private const string TOUCH = "TOUCH TO START";

    [SerializeField]
    private Slider _loadingBar;
    [SerializeField]
    private Button _btnNextScene;
    [SerializeField]
    private TextMeshProUGUI _txtInfo;

    private Tween _textTween;
    private int _dotCount;

    public event Action onNextSceneClicked;

    private void Awake()
    {
        _btnNextScene.onClick.AddListener(() => onNextSceneClicked?.Invoke());
        _btnNextScene.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _textTween?.Kill();
        _txtInfo.DOKill();
        _txtInfo.transform.DOKill();
    }

    public void StartLoadingAnimation()
    {
        _dotCount = 0;

        _textTween = DOTween.Sequence().AppendCallback(() =>
        {
            _dotCount = (_dotCount + 1) % 4;
            _txtInfo.text = LOADING + new string('.', _dotCount);
        }).AppendInterval(0.4f).SetLoops(-1);

        _txtInfo.transform.DOScale(1.05f, 0.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void UpdateProgressBar(float value)
    {
        _loadingBar.value = value;
    }

    public void TransitionToTouchToStart()
    {
        _textTween?.Kill();
        _txtInfo.transform.DOKill();

        _btnNextScene.gameObject.SetActive(true);

        _txtInfo.DOFade(0f, 0.3f).OnComplete(() =>
        {
            _txtInfo.text = TOUCH;
            _txtInfo.DOFade(1f, 0.4f).OnComplete(PlayTouchBlinkAnimation);
        });
    }

    private void PlayTouchBlinkAnimation()
    {
        _txtInfo.DOFade(0.3f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void DisableInteraction()
    {
        _btnNextScene.interactable = false;
        _txtInfo.DOKill();
        _txtInfo.transform.DOKill();
    }
}