using System;
using UnityEngine;

public abstract class IA_Base : MonoBehaviour
{
    public Action<IA_Base> OnDestroyIA = null;

    [SerializeField] protected bool bIsIADestroyed = false;
    [SerializeField] protected float fSpeedFactor = 1.0f;

    [SerializeField] protected SIA_Stats sStats = new SIA_Stats();
    [SerializeField] protected SGoalMovement_Data sMovementData = new SGoalMovement_Data();

    protected W_Path pathFinding = null;
    
    public bool IsIADestroyed => bIsIADestroyed;

    public void SetPath(W_Path _path) => pathFinding = _path;
    public void SetPerpendicularOffset(float _perpendicularOffset) => sMovementData.fPerpendicularOffset = _perpendicularOffset;

    public abstract void Tick(float _deltaTime);
}

public enum SIA_State
{
    Stand,
    Move,
    Attack
}