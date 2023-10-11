using UnityEngine;

public class IA_EnemyGround : IA_Enemy
{


    private void Start()
    {
        AskForNextGoal();
    }

    public override void Tick(float _deltaTime)
    {
        Move(_deltaTime);
    }

    private void Move(float _deltaTime)
    {
        Vector3 _goalPosition = sMovementData.vGoalPosition;

        Vector3 _movePosition = Vector3.MoveTowards(transform.position, _goalPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _movePosition;

        if (Vector3.Distance(_movePosition, _goalPosition) < sMovementData.fDistanceForNewGoal)
        {
            AskForNextGoal();

            if (sMovementData.iPathIndex == sMovementData.iLastPathIndex)
            {
                //Enemy reach the end
                bIsIADestroyed = true;
            }
        }
    }

    private void AskForNextGoal()
    {
        pathFinding.GetNextGoalPosition(ref sMovementData);
    }
}