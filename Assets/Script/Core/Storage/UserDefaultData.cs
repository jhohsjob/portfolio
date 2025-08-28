using UnityEngine;

[CreateAssetMenu(fileName = "UserDefaultData", menuName = "DefaultStorage/UserDefaultData")]
public class UserDefaultData : ScriptableObject
{
    public int level;
    public int exp;
    public int gold;
    public int mercenaryId;
}
