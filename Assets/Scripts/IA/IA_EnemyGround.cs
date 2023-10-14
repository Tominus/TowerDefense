using UnityEngine;

public class IA_EnemyGround : IA_Enemy
{


    private void Start()
    {
        AskForNextGoal();
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
}