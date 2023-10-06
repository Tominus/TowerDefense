using System;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class W_Path : MonoBehaviour
{
    [SerializeField] SplineContainer spline = null;
    [SerializeField] float splineLenght = 0.0f;
    [SerializeField, Range(0f, 0.99f)] float splineForwardOffset = 0.05f;


    private void Start()
    {
        splineLenght = spline.CalculateLength();
    }

    public void GetPosition(ref SMoveSpline_Data _splineData, float _moveOffset)
    {
        _splineData.fCurrentDistTravel += _moveOffset / splineLenght;

        _splineData.vCurrentPosition = spline.EvaluatePosition(_splineData.fCurrentDistTravel);
        _splineData.vForwardPosition = spline.EvaluatePosition(_splineData.fCurrentDistTravel + splineForwardOffset);

        Vector3 _tmpDirection = _splineData.vForwardPosition - _splineData.vCurrentPosition;

        _splineData.vPerpendicularPosition = _splineData.fPerpendicularOffset * Vector3.Cross(_tmpDirection.normalized, Vector3.forward);

        _splineData.vFinalPosition = _splineData.vPerpendicularPosition + _splineData.vCurrentPosition;
    }
}

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
}