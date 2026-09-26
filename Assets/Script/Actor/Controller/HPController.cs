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
    private float maxHP => _roleMaxHP + _elementMaxHP;
    private float currentHP { get; set; }

    public event Action<ChangeHPData> onChanged;

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

    public ChangeHPData GetCurrentData()
    {
        return new ChangeHPData(maxHP, currentHP, 0f, currentHP);
    }

    private void Adjust(float amount)
    {
        float beforeHP = currentHP;

        currentHP = Math.Clamp(currentHP + amount, 0, maxHP);

        var changeData = new ChangeHPData(maxHP, beforeHP, amount, currentHP);

        onChanged?.Invoke(changeData);
    }


    public void Clear()
    {
        onChanged = null;
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