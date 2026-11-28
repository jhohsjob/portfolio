using System;


public readonly struct ChangeHpData
{
    public readonly float maxHp;
    public readonly float beforeHp;
    public readonly float amount;
    public readonly float remainHp;
    public readonly float fillAmount;
    public readonly string text;

    public ChangeHpData(float maxHp, float beforeHp, float amount, float remainHp)
    {
        this.maxHp = maxHp;
        this.beforeHp = beforeHp;
        this.amount = amount;
        this.remainHp = remainHp;
        this.fillAmount = maxHp > 0f ? remainHp / maxHp : 0f;
        this.text = $"{remainHp} / {maxHp}";
    }
}

public class HpState
{
    private float _roleMaxHp;
    private float _elementMaxHp;
    private float maxHp => _roleMaxHp + _elementMaxHp;
    private float currentHp { get; set; }

    public event Action<ChangeHpData> onChanged;

    public void Enter(float maxHp)
    {
        _roleMaxHp = maxHp;
        _elementMaxHp = 0f;
        currentHp = maxHp;
    }

    public void Damage(float amount)
    {
        Adjust(-amount);
    }

    public void Recovery(float amount)
    {
        Adjust(amount);
    }

    public ChangeHpData GetCurrentData()
    {
        return new ChangeHpData(maxHp, currentHp, 0f, currentHp);
    }

    private void Adjust(float amount)
    {
        float beforeHp = currentHp;

        currentHp = Math.Clamp(currentHp + amount, 0, maxHp);

        var changeData = new ChangeHpData(maxHp, beforeHp, amount, currentHp);

        onChanged?.Invoke(changeData);
    }


    public void Clear()
    {
        onChanged = null;
        _roleMaxHp = 0f;
        _elementMaxHp = 0f;
        currentHp = 0f;
    }

    public void OnElementLevelUp()
    {
        _elementMaxHp += 10;
        Adjust(maxHp);
    }
}