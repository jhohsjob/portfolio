using TMPro;
using UnityEngine;


public class UIGold : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtGold;

    private void Awake()
    {
        Client.currencyService.onChanged += OnChangeGold;
    }

    private void Start()
    {
        _txtGold.text = Client.currencyService.Get(CurrencyType.Gold).ToString();
    }

    private void OnDestroy()
    {
        Client.currencyService.onChanged -= OnChangeGold;
    }

    private void OnChangeGold(CurrencyType type, int result)
    {
        if (type != CurrencyType.Gold)
        {
            return;
        }

        _txtGold.text = result.ToString();
    }
}
