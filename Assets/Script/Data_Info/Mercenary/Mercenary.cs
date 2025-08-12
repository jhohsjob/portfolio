public class Mercenary
{
    private MercenaryData _data;

    public string name => _data.roleName;
    public string description => _data.roleDescription;
    public int atk => 0;
    public float maxHp => _data.maxHP;
    public float moveSpeed => _data.moveSpeed;

    public string resourcePath => _data.resourcePath;

    public Mercenary(MercenaryData data)
    {
        _data = data.DeepCopy();
    }
}
