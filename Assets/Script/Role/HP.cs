public class ChangeHPData
{
    public float maxHP;
    public float beforeHP;
    public float amount;
    public float remainHP;
}

public class HP
{
    public float maxHP { get; private set; }
    public float currentHP { get; private set; }

    public delegate void DelegateChangeHP(ChangeHPData data);

    public void Init(float maxHP)
    {
        this.maxHP = maxHP;
        currentHP = maxHP;
    }

    public void AdjustHP(float amount, DelegateChangeHP changeHP)
    {
        var changeData = new ChangeHPData();
        changeData.maxHP = maxHP;
        changeData.beforeHP = currentHP;
        changeData.amount = amount;

        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        else if (currentHP < 0f)
        {
            currentHP = 0f;
        }

        changeData.remainHP = currentHP;

        changeHP(changeData);
    }
}
