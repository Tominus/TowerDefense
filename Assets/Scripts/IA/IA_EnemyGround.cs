using UnityEngine;

public class IA_EnemyGround : IA_Enemy
{


    private void Start()
    {
        AskForGoal();
    }

    public override void Tick(float _deltaTime)
    {
        Move(_deltaTime);
    }

    private void Move(float _deltaTime)
    {
        Vector3 _currentPosition = transform.position;
        Vector3 _goalPosition = sSplineData.vGoalPosition;

        Vector3 _movePosition = Vector3.MoveTowards(_currentPosition, _goalPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _movePosition;

        if (Vector3.Distance(_movePosition, _goalPosition) < sSplineData.fDistanceForNewGoal)
        {
            AskForGoal();
        }

        if (sSplineData.fCurrentDistTravel >= 1.0f)
        {
            bIsIADestroyed = true;
        }
    }

    private void AskForGoal()
    {
        pathFinding.GetGoalPosition(ref sSplineData, transform.position);
    }
}