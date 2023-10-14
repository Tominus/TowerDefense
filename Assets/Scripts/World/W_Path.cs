using System;
using System.Collections.Generic;
using UnityEngine;

public class W_Path : MonoBehaviour
{
    [SerializeField] List<Transform> allPoints = new List<Transform>();

    [SerializeField] Vector3 vStartPosition = Vector3.zero;
    [SerializeField] Vector3 vEndPosition = Vector3.zero;
    [SerializeField] Vector3 vStartDirection = Vector3.zero;

    [SerializeField] bool bDeactivatePointsRender = false;

    public Vector3 StartPosition => vStartPosition;
    public Vector3 StartDirection => vStartDirection;
    public Vector3 EndPosition => vEndPosition;

    private void Start()
    {
        InitPointsPosition();
        InitPointsRender();
    }

    private void InitPointsPosition()
    {
        int _count = allPoints.Count;
        if (_count < 2)
        {
            Debug.Log("Need more point in Path");
            return;
        }

        Transform _startPoint = allPoints[0];
        vStartPosition = _startPoint.position;
        vStartDirection = _startPoint.up;
        vEndPosition = allPoints[_count - 1].position;
    }
    private void InitPointsRender()
    {
        if (!bDeactivatePointsRender) return;

        int _count = allPoints.Count;
        for (int i = 0; i < _count; ++i)
        {
            W_PathPoint _pathPoint = allPoints[i].GetComponent<W_PathPoint>();
            _pathPoint.DeactivateRender();
        }
    }

    public bool GetNextGoalPosition(ref SGoalMovement_Data _data)
    {
        if (_data.iPathIndex >= allPoints.Count)
        {
            //They are at the latest position
            return false;
        }

        Transform _currentPoint = allPoints[_data.iPathIndex];

        _data.vGoalPosition = _data.fPerpendicularOffset * _currentPoint.up + _currentPoint.position;

        return true;
    }
}

[Serializable]
public struct SGoalMovement_Data
{
    public int iPathIndex;
    public int iLastPathIndex;
    public float fPerpendicularOffset;
    public float fDistanceForNewGoal;
    public Vector3 vGoalPosition;

    public SGoalMovement_Data(int _pathIndex, int _lastPathIndex, float _perpendicularOffset, float _distanceForNewGoal, Vector3 _goalPosition)
    {
        iPathIndex = _pathIndex;
        iLastPathIndex = _lastPathIndex;
        fPerpendicularOffset = _perpendicularOffset;
        fDistanceForNewGoal = _distanceForNewGoal;
        vGoalPosition = _goalPosition;
    }
}