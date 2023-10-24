using UnityEngine;

public class P_ArrowProjectile : P_BaseProjectile
{


    private void Start()
    {
        P_ProjectileManager.Instance.AddProjectile(this);

        OnTick += MoveToTarget;
    }

    public override void Tick(float _deltaTime)
    {
        OnTick.Invoke(_deltaTime);
    }

    protected override void ChangeMovementMove(IA_Base _target)
    {
        base.ChangeMovementMove(_target);

        sProjectileData.vTargetLastPosition = _target.transform.position;

        OnTick = null;
        OnTick += MoveToPosition;
    }

    private void MoveToPosition(float _deltaTime)
    {
        Vector3 _targetPosition = sProjectileData.vTargetLastPosition;
        Vector3 _movePosition = Vector3.MoveTowards(transform.position, sProjectileData.vTargetLastPosition, _deltaTime * sProjectileData.fProjectileSpeed);
        transform.position = _movePosition;

        if (Vector3.Distance(_movePosition, _targetPosition) < sProjectileData.fProjectileReachDamage)
        {
            bIsDestroyed = true;
            OnTick = null;
        }
    }
    private void MoveToTarget(float _deltaTime)
    {
        IA_Base _iaBase = sProjectileData.target;
        Vector3 _targetPosition = _iaBase.transform.position;

        Vector3 _movePosition = Vector3.MoveTowards(transform.position, _targetPosition, _deltaTime * sProjectileData.fProjectileSpeed);
        transform.position = _movePosition;

        if (Vector3.Distance(_movePosition, _targetPosition) < sProjectileData.fProjectileReachDamage)
        {
            DoDamageToTarget(_iaBase);
        }
    }

    private void DoDamageToTarget(IA_Base _iaBase)
    {
        _iaBase.TakeDamage(sProjectileData.fAttackDamage, sProjectileData.eAttackType);
        bIsDestroyed = true;
        OnTick = null;
    }
}