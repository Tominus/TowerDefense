using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class W_Path : MonoBehaviour
{
    [SerializeField] SplineContainer spline = null;
    [SerializeField] float fSplineLenght = 0.0f;
    [SerializeField, Range(2, 64)] int iResolution = 4;
    [SerializeField, Range(2, 64)] int iIteration = 2;

    Spline thisSpline = null;

    private void Start()
    {
        fSplineLenght = spline.CalculateLength();
        thisSpline = spline.Spline;
    }

    /*public void GetPosition(ref SMoveSpline_Data _splineData, float _moveOffset)
    {
        _splineData.fCurrentDistTravel += _moveOffset / splineLenght;

        _splineData.vCurrentPosition = spline.EvaluatePosition(_splineData.fCurrentDistTravel);
        _splineData.vForwardPosition = spline.EvaluatePosition(_splineData.fCurrentDistTravel + splineForwardOffset);
        
        Vector3 _tmpDirection = _splineData.vForwardPosition - _splineData.vCurrentPosition;

        _splineData.vPerpendicularPosition = _splineData.fPerpendicularOffset * Vector3.Cross(_tmpDirection.normalized, Vector3.forward);

        _splineData.vFinalPosition = _splineData.vPerpendicularPosition + _splineData.vCurrentPosition;
    }*/

    public float GetNearestSplinePoint(Vector3 _position)
    {
        SplineUtility.GetNearestPoint(thisSpline, _position, out float3 _nearestPosition, out float _time, iResolution, iIteration);
        Debug.Log(_time);
        return _time;
    }
    public void GetGoalPosition(ref SMoveSpline_Data _data, Vector3 _currentPosition)
    {
        float _currentTime = GetNearestSplinePoint(_currentPosition);
        _data.fCurrentDistTravel = _data.fNearestSplinePoint = _currentTime;

        Vector3 _position = spline.EvaluatePosition(_currentTime);
        _currentTime += _data.fSplineFwdOffset;
        Vector3 _fwdPosition = spline.EvaluatePosition(_currentTime);

        Vector3 _perpendicularPosition = _data.fPerpendicularOffset * Vector3.Cross((_fwdPosition - _position).normalized, Vector3.forward);
        _data.vGoalPosition = _perpendicularPosition + _fwdPosition;
    }
}

[Serializable]
public struct SMoveSpline_Data
{
    public Vector3 vGoalPosition;
    public float fNearestSplinePoint;
    public float fSplineFwdOffset;
    public float fCurrentDistTravel;   //Not used
    public float fPerpendicularOffset;
    public float fDistanceForNewGoal;

    public SMoveSpline_Data(Vector3 _goalPosition, float _nearestSplinePoint, float _splineFwdOffset, float _currentDistTravel, float _perpendicularOffset,
                            float _distanceForNewGoal)
    {
        vGoalPosition = _goalPosition;
        fNearestSplinePoint = _nearestSplinePoint;
        fSplineFwdOffset = _splineFwdOffset;
        fCurrentDistTravel = _currentDistTravel;
        fPerpendicularOffset = _perpendicularOffset;
        fDistanceForNewGoal = _distanceForNewGoal;
    }
}
/*
[Serializable]
public struct SMoveSpline_Data
{
    public Vector3 vCurrentPosition;
    public Vector3 vFinalPosition;
    public Vector3 vForwardPosition;
    public Vector3 vPerpendicularPosition;
    public float fCurrentDistTravel;
    public float fPerpendicularOffset;

    public SMoveSpline_Data(Vector3 _currentPosition, Vector3 _finalPosition, Vector3 _forwardPosition, Vector3 _perpendicularPosition, float _currentDistTravel, float _perpendicularOffset)
    {
        vCurrentPosition = _currentPosition;
        vFinalPosition = _finalPosition;
        vForwardPosition = _forwardPosition;
        vPerpendicularPosition = _perpendicularPosition;
        fCurrentDistTravel = _currentDistTravel;
        fPerpendicularOffset = _perpendicularOffset;
    }
}*/