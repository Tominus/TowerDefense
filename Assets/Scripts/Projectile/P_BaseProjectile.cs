using System;
using UnityEngine;

public abstract class P_BaseProjectile : MonoBehaviour
{
    protected Action<float> OnTick = null;

    [SerializeField] protected bool bIsDestroyed = false;
    [SerializeField] protected SProjectile_Data sProjectileData = new SProjectile_Data();

    public bool IsDestroyed => bIsDestroyed;

    public abstract void Tick(float _deltaTime);

    public void SetProjectileData(IA_Base _target, float _projectileSpeed, EAttack_Type _attackType, float _attackDamage)
    {
        sProjectileData.target = _target;
        _target.OnDestroyIA += ChangeMovementMove;
        sProjectileData.fProjectileSpeed = _projectileSpeed;
        sProjectileData.eAttackType = _attackType;
        sProjectileData.fAttackDamage = _attackDamage;
    }

    protected virtual void ChangeMovementMove(IA_Base _target)
    {
        _target.OnDestroyIA -= ChangeMovementMove;
    }

    private void OnDestroy()
    {
        OnTick = null;
    }
}

[Serializable]
public struct SProjectile_Data
{
    public IA_Base target;
    public Vector3 vTargetLastPosition;
    public float fProjectileSpeed;
    public float fProjectileReachDamage;

    public EAttack_Type eAttackType;
    public float fAttackDamage;
}