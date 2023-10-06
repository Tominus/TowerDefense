using UnityEngine;

public class IA_EnemyGround : IA_Enemy
{
    [SerializeField] Vector3 lastPostion = Vector3.zero;

    private void Start()
    {
        lastPostion = transform.position;
    }

    public override void Tick(float _deltaTime)
    {
        Move(_deltaTime);
    }

    private void Move(float _deltaTime)
    {
        Vector3 _currentPosition = transform.position;
        float _deltaPosition = Vector3.Distance(_currentPosition, lastPostion);
        lastPostion = _currentPosition;

        if (_deltaPosition < 0.01f)
        {
            _deltaPosition = 0.01f;
        }

        pathFinding.GetPosition(ref sSplineData, _deltaPosition);

        Vector3 _movePosition = Vector3.MoveTowards(_currentPosition, sSplineData.vFinalPosition, _deltaTime * sStats.fMoveSpeed * fSpeedFactor);
        transform.position = _movePosition;

        if (sSplineData.fCurrentDistTravel >= 1.0f)
        {
            bIsIADestroyed = true;
        }
    }
}