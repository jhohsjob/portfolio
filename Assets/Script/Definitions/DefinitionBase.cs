using UnityEngine;


public abstract class DefinitionBase : ScriptableObject
{
    [SerializeField]
    private int _id;

    public int id
    {
        get => _id;
        set => _id = value;
    }
}