using System;
using UnityEngine;

public abstract class IA_Base : MonoBehaviour
{
    public Action<IA_Base> OnDestroyIA = null;

    [SerializeField] protected bool bIsIADestroyed = false;
    [SerializeField] protected float fSpeedFactor = 1.0f;

    [SerializeField] protected SIA_Stats sStats = new SIA_Stats();
    
    public bool IsIADestroyed => bIsIADestroyed;

    public abstract void Tick(float _deltaTime);
}