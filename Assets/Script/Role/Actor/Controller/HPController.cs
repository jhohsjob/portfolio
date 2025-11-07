using System;


public readonly struct ChangeHPData
{
    public readonly float maxHP;
    public readonly float beforeHP;
    public readonly float amount;
    public readonly float remainHP;
    public readonly float fillAmount;
    public readonly string text;

    public ChangeHPData(float maxHP, float beforeHP, float amount, float remainHP)
    {
        this.maxHP = maxHP;
        this.beforeHP = beforeHP;
        this.amount = amount;
        this.remainHP = remainHP;
        this.fillAmount = maxHP > 0f ? remainHP / maxHP : 0f;
        this.text = $"{remainHP} / {maxHP}";
    }
}

public class HPController
{
    private float _roleMaxHP;
    private float _elementMaxHP;
    public float maxHP => _roleMaxHP + _elementMaxHP;
    public float currentHP { get; private set; }

    protected Action<ChangeHPData> _cbChange;
    public event Action<ChangeHPData> cbChange
    {
        add { _cbChange -= value; _cbChange += value; }
        remove { _cbChange -= value; }
    }

    public void Enter(float maxHP)
    {
        _roleMaxHP = maxHP;
        _elementMaxHP = 0f;
        currentHP = maxHP;
    }

    public void Damage(float amount)
    {
        Adjust(-amount);
    }

    public void Recovery(float amount)
    {
        Adjust(amount);
    }

    private void Adjust(float amount)
    {
        float beforeHP = currentHP;

        currentHP = Math.Clamp(currentHP + amount, 0, maxHP);

        var changeData = new ChangeHPData(maxHP, beforeHP, amount, currentHP);

        _cbChange?.Invoke(changeData);
    }

    public void Clear()
    {
        _cbChange = null;
        _roleMaxHP = 0f;
        _elementMaxHP = 0f;
        currentHP = 0f;
    }

    public void OnElementLevelUp()
    {
        _elementMaxHP += 10;
        Adjust(maxHP);
    }
}
