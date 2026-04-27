using System.Threading.Tasks;
using UnityEngine;


public struct RewardResult
{
    public bool success;
    public string message;

    public RewardType type;
    public int id;
    public int amount;

    public static RewardResult Success(RewardType type, int id = 0, int amount = 0, string message = "")
    {
        return new RewardResult
        {
            success = true,
            type = type,
            id = id,
            amount = amount,
            message = message
        };
    }

    public static RewardResult Fail(string message)
    {
        return new RewardResult
        {
            success = false,
            message = message
        };
    }
}

public abstract class RewardBase : ScriptableObject
{
    public abstract RewardResult CanGive();
    public abstract Task Apply();
}
