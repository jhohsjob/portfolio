using System;


[Serializable]
public class StatDefinition
{
    public OffensiveStatDefinition offensive;
    public DefensiveStatDefinition defensive;
    public UtilityStatDefinition utility;
}


[Serializable]
public class OffensiveStatDefinition
{
    public GrowthStatDefinition attack;
    public GrowthStatDefinition criticalChance;
    public GrowthStatDefinition criticalDamage;
}


[Serializable]
public class DefensiveStatDefinition
{
    public GrowthStatDefinition maxHP;
    public GrowthStatDefinition defense;
}


[Serializable]
public class UtilityStatDefinition
{
    public float moveSpeed;
}


[Serializable]
public class GrowthStatDefinition
{
    public float value;
    public float growth;
}