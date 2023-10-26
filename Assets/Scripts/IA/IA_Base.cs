using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class IA_Base : MonoBehaviour
{
    public Action<IA_Base> OnDestroyIA = null;
    protected Action<float> OnTickIA = null;

    [SerializeField] protected bool bIsIADestroyed = false;
    [SerializeField] protected float fSpeedFactor = 1.0f;

    [SerializeField] protected SIA_Stats sStats = new SIA_Stats();
    [SerializeField] protected SGoalMovement_Data sMovementData = new SGoalMovement_Data();
    [SerializeField] protected EIA_State eState = EIA_State.Move;
    [SerializeField] protected EIA_Type eIAType = EIA_Type.Ground;
    [SerializeField] protected EIA_AttackStyle eIAAttackStyle = EIA_AttackStyle.Ground;

    protected W_Path pathFinding = null;
    
    public bool IsIADestroyed => bIsIADestroyed;
    public EIA_State State => eState;
    public EIA_Type IAType => eIAType;
    public EIA_AttackStyle IAAttackStyle => eIAAttackStyle;

    public void SetPath(W_Path _path) => pathFinding = _path;
    public void SetPerpendicularOffset(float _perpendicularOffset) => sMovementData.fPerpendicularOffset = _perpendicularOffset;

    public abstract void Tick(float _deltaTime);

    public void TakeDamage(float _amount, EAttack_Type _attackType)
    {
        sStats.fCurrentLife -= _amount;
        if (sStats.fCurrentLife <= 0f)
        {
            OnDestroyIA?.Invoke(this);
            bIsIADestroyed = true;
        }
    }

    protected virtual void OnDestroy()
    {
        OnDestroyIA = null;
        OnTickIA = null;
    }
}

public enum EIA_State
{
    Stand,
    Move,
    MoveToRallyPoint,
    Attack,
    AttackDistance,
}

public enum EIA_Type
{
    Ground,
    Air,
}

public enum EIA_AttackStyle
{
    Ground,
    Air,
    Distance,
}