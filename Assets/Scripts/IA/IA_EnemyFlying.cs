using System;
using UnityEngine;

public class IA_EnemyFlying : IA_Enemy
{
    [SerializeField] SFlyingMove_Data sFlyingMoveData = new SFlyingMove_Data();

    public Transform SpriteSocket => sFlyingMoveData.spriteSocket;

    private void Start()
    {
        AskForNextGoal();
        SetupSpriteSocket();
    }

    protected override void Move(float _deltaTime)
    {
        Vector3 _goalPosition = sMovementData.vGoalPosition;

        Vector3 _movePosition = Vector3.MoveTowards(transform.position, _goalPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _movePosition;

        if (Vector3.Distance(_movePosition, _goalPosition) < sMovementData.fDistanceForNewGoal)
        {
            ++sMovementData.iPathIndex;

            AskForNextGoal();
        }
    }

    protected void CheckForAlly(float _deltaTime)
    {
        //TODO if AttackType == Distance it will attack GroundAlly
    }

    protected void SetupSpriteSocket()
    {
        sFlyingMoveData.spriteSocket.position = transform.position + new Vector3(0, sFlyingMoveData.fFlyingHeight, 0);
    }
}

[Serializable]
public struct SFlyingMove_Data
{
    public float fFlyingHeight;
    public Transform spriteSocket;

    public SFlyingMove_Data(float _flyingHeight, Transform _spriteSocket)
    {
        fFlyingHeight = _flyingHeight;
        spriteSocket = _spriteSocket;
    }
}