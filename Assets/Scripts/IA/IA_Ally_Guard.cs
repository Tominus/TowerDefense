using System.Collections.Generic;
using UnityEngine;

public class IA_Ally_Guard : IA_Ally
{
    [SerializeField] List<IA_Enemy> allEnemyInRange = new List<IA_Enemy>();
    [SerializeField] IA_Enemy fightingEnemy = null;

    protected override void Start()
    {
        base.Start();
        OnTickIA += CheckForEnemy;
        eState = EIA_State.Stand;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sEnemyDetectionData.fCheckRange);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        IA_Enemy _enemy = _other.GetComponent<IA_Enemy>();
        if (_enemy)
        {
            //TODO check if enemy is Air or Ground
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

    //TODO if range
    //TODO Check if Ground or Air 
    protected void CheckForEnemy(float _deltaTime)
    {
        Vector3 _position = transform.position;
        IA_Enemy _nearestEnemy = null;
        float _minDistance = float.MaxValue;

        int _count = allEnemyInRange.Count;
        for (int i = 0; i < _count; ++i)
        {
            IA_Enemy _currentEnemy = allEnemyInRange[i];

            if (_currentEnemy.State == EIA_State.Move)
            {
                float _currentDistance = Vector3.Distance(_position, _currentEnemy.transform.position);
                if (_currentDistance < _minDistance)
                {
                    _minDistance = _currentDistance;
                    _nearestEnemy = _currentEnemy;
                }
            }
        }

        if (_nearestEnemy)
        {
            OnTickIA = null;
            OnTickIA += MoveTowardEnemy;

            eState = EIA_State.Move;

            fightingEnemy = _nearestEnemy;
            fightingEnemy.WaitForFight(this);
            fightingEnemy.OnDestroyIA += FinishFight;
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
    //TODO Go back to his flag or fight another enemy
    public void FinishFight(IA_Base _enemy)
    {
        fightingEnemy.OnDestroyIA -= FinishFight;
        fightingEnemy = null;

        eState = EIA_State.Stand;

        OnTickIA = null;
        OnTickIA += CheckForEnemy;
    }
}