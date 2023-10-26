using System.Collections.Generic;
using UnityEngine;

public class IA_Ally_Guard : IA_Ally
{
    [SerializeField] List<IA_Enemy> allEnemyInRange = new List<IA_Enemy>();
    [SerializeField] IA_Enemy fightingEnemy = null;
    [SerializeField] bool bDrawDebug = false;
    [SerializeField] bool bDrawDistanceDebug = true;
    [SerializeField] float fCheckCurrentTime = 0f;
    [SerializeField] float fCheckMaxTime = 0.1f;
    float fRallyPointEnemyRange = 1.5f;

    //Position of the rallypoint
    Vector3 fRallyPointEnemyPosition = Vector3.zero;

    //Position of the rally point offsetted for the guard
    [SerializeField] Vector3 vRallyPosition = Vector3.zero;

    protected override void Start()
    {
        base.Start();

        OnTickIA += CheckForEnemy;
        eState = EIA_State.Stand;
    }
    private void OnDrawGizmos()
    {
        if (bDrawDebug)
        {
            Vector3 _position = transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_position, sEnemyDetectionData.fCheckRange);

            if (bDrawDistanceDebug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_position, sStats.fAttackDistanceMaxRange);
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(_position, sStats.fAttackDistanceMinRange);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        IA_Enemy _enemy = _other.GetComponent<IA_Enemy>();
        if (_enemy)
        {
            if (_enemy.IAType == EIA_Type.Air && eIAAttackStyle == EIA_AttackStyle.Ground)
            {
                return;
            }

            allEnemyInRange.Add(_enemy);
        }
    }
    private void OnTriggerExit2D(Collider2D _other)
    {
        IA_Enemy _enemy = _other.GetComponent<IA_Enemy>();
        if (_enemy)
        {
            int _count = allEnemyInRange.Count;
            for (int i = 0; i < _count; ++i)
            {
                if (allEnemyInRange[i] == _enemy)
                {
                    allEnemyInRange.RemoveAt(i);
                    return;
                }
            }
        }
    }

    protected void StartChecking()
    {
        OnTickIA += CheckForEnemy;
        OnTickIA += MoveToCheckPoint;
    }
    protected void CheckForEnemy(float _deltaTime)
    {
        fCheckCurrentTime += _deltaTime;
        if (fCheckCurrentTime < fCheckMaxTime)
        {
            return;
        }

        fCheckCurrentTime = 0f;

        Vector3 _position = transform.position;
        IA_Enemy _nearestEnemy = null;
        float _minDistance = float.MaxValue;

        int _count = allEnemyInRange.Count;
        for (int i = 0; i < _count; ++i)
        {
            IA_Enemy _currentEnemy = allEnemyInRange[i];

            if (_currentEnemy.State == EIA_State.Move)
            {
                Vector3 _enemyPosition = _currentEnemy.transform.position;

                //Check enemy in range of rally point.
                if (Vector3.Distance(_enemyPosition, fRallyPointEnemyPosition) < fRallyPointEnemyRange)
                {
                    //Check the nearest enemy from Guard.
                    float _currentDistance = Vector3.Distance(_position, _enemyPosition);
                    if (_currentDistance < _minDistance)
                    {
                        _minDistance = _currentDistance;
                        _nearestEnemy = _currentEnemy;
                    }
                }
            }
        }

        if (_nearestEnemy)
        {
            OnTickIA = null;

            //Move to enemy position and attack
            if (eIAAttackStyle == EIA_AttackStyle.Ground)
            {
                OnTickIA += MoveTowardEnemy;

                eState = EIA_State.Move;

                fightingEnemy = _nearestEnemy;
                fightingEnemy.WaitForFight(this);
                fightingEnemy.OnDestroyIA += FinishFight;
            }

            else if (eIAAttackStyle == EIA_AttackStyle.Distance)
            {
                fightingEnemy = _nearestEnemy;

                if (sStats.fAttackDistanceMaxRange < _minDistance)
                {
                    //Move toward enemy until he can attack
                    OnTickIA += MoveTowardEnemyWithRange;
                    eState = EIA_State.Move;

                    fightingEnemy.OnDestroyIA += StopMoveTowardEnemyWithRange;
                }
                else if (sStats.fAttackDistanceMinRange < _minDistance)
                {
                    //Enemy too close

                    if (fightingEnemy.IAType == EIA_Type.Ground)
                    {
                        OnTickIA += MoveTowardEnemy;

                        eState = EIA_State.Move;

                        fightingEnemy.WaitForFight(this);
                        fightingEnemy.OnDestroyIA += FinishFight;
                    }
                    else // EnemyType == Air
                    {
                        StartAttackDistance();
                    }
                }
                else
                {
                    StartAttackDistance();
                }
            }
            else
            {
                Debug.Log("Attack Air style not implemented yet");
            }
        }

        if (OnTickIA == null)
        {
            Debug.Log("IA_Ally_Guard::CheckForEnemy -> IA is doing nothing");
        }
    }

    protected void MoveTowardEnemy(float _deltaTime)
    {
        Vector3 _enemyPosition = fightingEnemy.transform.position;
        Vector3 _position = Vector3.MoveTowards(transform.position, _enemyPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _position;

        if (Vector3.Distance(_position, _enemyPosition) < 0.1f)
        {
            StartFight();
        }
    }
    protected void MoveTowardEnemyWithRange(float _deltaTime)
    {
        Vector3 _enemyPosition = fightingEnemy.transform.position;
        Vector3 _position = Vector3.MoveTowards(transform.position, _enemyPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _position;

        if (Vector3.Distance(fRallyPointEnemyPosition, _enemyPosition) > fRallyPointEnemyRange)
        {
            //Enemy too far from rally point
            fightingEnemy.OnDestroyIA -= StopMoveTowardEnemyWithRange;
            FinishAttackDistance(fightingEnemy);
            return;
        }
        if (Vector3.Distance(_position, _enemyPosition) < sStats.fAttackDistanceMaxRange)
        {
            fightingEnemy.OnDestroyIA -= StopMoveTowardEnemyWithRange;
            StartAttackDistance();
        }
    }

    protected void StartFight()
    {
        OnTickIA = null;
        OnTickIA += UpdateFight;
        eState = EIA_State.Attack;
        fightingEnemy.StartFight();
    }
    protected void UpdateFight(float _deltaTime)
    {
        sStats.fAttackCooldown += _deltaTime * fSpeedFactor;
        
        if (sStats.fAttackCooldown >= sStats.fAttackSpeed)
        {
            sStats.fAttackCooldown = 0f;
            fightingEnemy.TakeDamage(sStats.fAttackDamage, sStats.eAttackType);
        }
    }
    public void FinishFight(IA_Base _enemy)
    {
        fightingEnemy.OnDestroyIA -= FinishFight;
        fightingEnemy = null;

        eState = EIA_State.Stand;

        sStats.fAttackCooldown = 0f;

        OnTickIA = null;

        if (!bIsIADestroyed)
            Invoke(nameof(StartChecking), 0.1f);
    }

    protected void StartAttackDistance()
    {
        OnTickIA = null;
        OnTickIA += UpdateAttackDistance;
        eState = EIA_State.AttackDistance;
        fightingEnemy.OnDestroyIA += FinishAttackDistance;
    }
    protected void UpdateAttackDistance(float _deltaTime)
    {
        sStats.fAttackCooldown += _deltaTime * fSpeedFactor;

        if (sStats.fAttackCooldown >= sStats.fAttackSpeed)
        {
            sStats.fAttackCooldown = 0f;

            P_BaseProjectile _baseProjectile = Instantiate(sStats.baseProjectilePrefab, transform.position, Quaternion.identity);
            _baseProjectile.SetProjectileData(fightingEnemy, sStats.fProjectileSpeed, sStats.eAttackType, sStats.fAttackDamage);

            Vector3 _enemyPosition = fightingEnemy.transform.position;
            if (Vector3.Distance(transform.position, _enemyPosition) > sStats.fAttackDistanceMaxRange)
            {
                //Enemy too far from guard
                if (Vector3.Distance(fRallyPointEnemyPosition, _enemyPosition) > fRallyPointEnemyRange)
                {
                    FinishAttackDistance(fightingEnemy);
                }
                else
                {
                    //Move toward enemy
                    OnTickIA = null;
                    OnTickIA += MoveTowardEnemyWithRange;
                    fightingEnemy.OnDestroyIA += StopMoveTowardEnemyWithRange;
                }
            }
        }
    }
    protected void FinishAttackDistance(IA_Base _enemy)
    {
        OnTickIA = null;
        fightingEnemy.OnDestroyIA -= FinishAttackDistance;
        fightingEnemy.OnDestroyIA -= StopMoveTowardEnemyWithRange;
        fightingEnemy = null;
        eState = EIA_State.Stand;
        sStats.fAttackCooldown = 0f;

        if (!bIsIADestroyed)
            Invoke(nameof(StartChecking), 0.1f);
    }
    protected void StopMoveTowardEnemyWithRange(IA_Base _enemy)
    {
        OnTickIA = null;
        fightingEnemy.OnDestroyIA -= StopMoveTowardEnemyWithRange;
        fightingEnemy.OnDestroyIA -= FinishAttackDistance;
        fightingEnemy = null;
        eState = EIA_State.Stand;

        if (!bIsIADestroyed)
            Invoke(nameof(StartChecking), 0.1f);
    }

    public void ForceMoveToCheckPoint(Vector3 _position)
    {
        vRallyPosition = _position;

        if (eState == EIA_State.Move || eState == EIA_State.Attack)
        {
            fightingEnemy.FinishFight(this);
            fightingEnemy.OnDestroyIA -= FinishFight;
            fightingEnemy = null;
        }

        OnTickIA = null;
        OnTickIA += MoveToCheckPoint;
        eState = EIA_State.MoveToRallyPoint;
    }
    protected void MoveToCheckPoint(float _deltaTime)
    {
        Vector3 _position = Vector3.MoveTowards(transform.position, vRallyPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _position;

        if (Vector3.Distance(_position, vRallyPosition) < 0.1f)
        {
            OnTickIA = null;
            OnTickIA += CheckForEnemy;
            eState = EIA_State.Stand;
        }
    }

    public void SetRallyPointEnemyRange(float _rallyPointEnemyRange, Vector3 _rallyPointEnemyPosition)
    {
        fRallyPointEnemyRange = _rallyPointEnemyRange;
        fRallyPointEnemyPosition = _rallyPointEnemyPosition;
    }
}