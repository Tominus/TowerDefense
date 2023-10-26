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
    public float fAttackCooldown;

    public float fAttackDistanceMaxRange;
    public float fAttackDistanceMinRange;
    public P_BaseProjectile baseProjectilePrefab;
    public float fProjectileSpeed;

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
    Immune =  0,   // x0.0
    Extreme = 20,  // x0.2
    Strong =  40,  // x0.4
    Good =    60,  // x0.6
    Normal =  80,  // x0.8
    Small =   100, // x1.0
    Weak =    125, // x1.25
    Nemesis = 150, // x1.5
}