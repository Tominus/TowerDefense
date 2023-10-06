using System;
using UnityEngine;

[Serializable]
public struct SIA_Stats
{
    public float fMaxLife;
    public float fCurrentLife;

    public float fMoveSpeed;

    public EAttack_Type eAttackType;
    public float fAttackDamage;
    public float fAttackSpeed;

    public EDamage_Resistance ePhysicResistance;
    public EDamage_Resistance eMagicResistance;
}

public enum EAttack_Type
{
    Physic,
    Magic
}

public enum EDamage_Resistance
{
             // Damage multiplier
    Nemesis, // x1.5
    Weak,    // x1.25
    Normal,  // x1.0
    Small,   // x0.8
    Good,    // x0.6
    Strong,  // x0.4
    Extreme, // x0.2
    Immune   // x0.0
}