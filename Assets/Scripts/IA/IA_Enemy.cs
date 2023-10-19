using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class IA_Enemy : IA_Base
{
    [SerializeField] protected IA_Ally fightingAlly = null;

    public override void Tick(float _deltaTime)
    {
        OnTickIA?.Invoke(_deltaTime);
    }

    protected virtual void AskForNextGoal()
    {
        OnTickIA = null;

        if (!pathFinding.GetNextGoalPosition(ref sMovementData))
        {
            //Enemy reach the end
            bIsIADestroyed = true;
            OnDestroyIA?.Invoke(this);
        }
        else
        {
            eState = EIA_State.Move;
            OnTickIA += Move;
        }
    }
    protected virtual void Move(float _deltaTime)
    {
        //...
    }

    public void WaitForFight(IA_Ally _ally)
    {
        OnTickIA = null;
        fightingAlly = _ally;
        fightingAlly.OnDestroyIA += FinishFight;
        eState = EIA_State.Stand;
    }
    public void StartFight()
    {
        OnTickIA = null;
        OnTickIA += UpdateFight;
        eState = EIA_State.Attack;
    }
    protected virtual void UpdateFight(float _deltaTime)
    {
        sStats.fAttackCooldown += _deltaTime * fSpeedFactor;

        if (sStats.fAttackCooldown >= sStats.fAttackSpeed)
        {
            sStats.fAttackCooldown = 0f;
            fightingAlly.TakeDamage(sStats.fAttackDamage, sStats.eAttackType);
        }
    }
    public void FinishFight(IA_Base _ally)
    {
        fightingAlly.OnDestroyIA -= FinishFight;
        fightingAlly = null;

        OnTickIA = null;

        if (!bIsIADestroyed)
            Invoke(nameof(AskForNextGoal), 0.1f);
    }
}