using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleTop : MonoBehaviour
{
    [SerializeField]
    private Button _btnPause;

    [SerializeField]
    private TextMeshProUGUI _txtGold;

    [SerializeField]
    private TextMeshProUGUI _txtElementWater;
    [SerializeField]
    private TextMeshProUGUI _txtElementNature;
    [SerializeField]
    private TextMeshProUGUI _txtElementFire;

    public event Action onClickPause;

    private void Awake()
    {
        _btnPause.onClick.AddListener(() =>
        {
            onClickPause?.Invoke();
        });
    }

    public void SetGold(int gold)
    {
        _txtGold.text = gold.ToString();
    }

    public void SetElement(ElementType type, int value)
    {
        switch (type)
        {
            case ElementType.Water:
                _txtElementWater.text = value.ToString();
                break;

            case ElementType.Nature:
                _txtElementNature.text = value.ToString();
                break;

            case ElementType.Fire:
                _txtElementFire.text = value.ToString();
                break;
        }
    }
}