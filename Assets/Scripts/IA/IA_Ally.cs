using System;
using UnityEngine;


public abstract class IA_Ally : IA_Base
{
    [SerializeField] protected SEnemyDetection_Data sEnemyDetectionData = new SEnemyDetection_Data();
    [SerializeField] protected CircleCollider2D circleCollider = null;

    protected virtual void Start()
    {
        circleCollider.radius = sEnemyDetectionData.fCheckRange;
        IA_Manager.Instance.AddIA(this);
    }

    public override void Tick(float _deltaTime)
    {
        OnTickIA?.Invoke(_deltaTime);
    }
}


[Serializable]
public struct SEnemyDetection_Data
{
    [SerializeField] public float fCheckInterval;
    [SerializeField] public float fCheckRange;
    //float fDistanceForFight; // Used for ranged Guard

    public SEnemyDetection_Data(float _checkInterval, float _checkRange)
    {
        fCheckInterval = _checkInterval;
        fCheckRange = _checkRange;
    }
}