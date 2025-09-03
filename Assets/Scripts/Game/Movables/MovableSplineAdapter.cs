using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Controllers;
using UnityEngine;

/// <summary>
/// Interface pour le spline controller
/// </summary>
[RequireComponent(typeof(SplineController))]
public class MovableSplineAdapter : MonoBehaviour {
    private SplineController _splineController;
    private CurvySpline _cachedSpline;
    private Transform _cachedTransform;
    private float _xOffset;

    private void Awake() {
        _splineController = GetComponent<SplineController>();
        _cachedTransform = transform;
    }

    public void SetInitialPosition(float f) {
        _splineController.Position = f;
    }

    public void SetSpeed(float s) {
        _splineController.MovementDirection = (s > 0) ? MovementDirection.Forward : MovementDirection.Backward;
        _splineController.Speed = Mathf.Abs(s);
    }

    public void SetXOffset(float x) {
        _xOffset = x;
        _splineController.OffsetRadius = _xOffset;
    }

    public void SetSpline(CurvySpline spline) {
        _cachedSpline = spline;
        _splineController.Spline = spline;
    }

    public void RemoveSpline() {
        _splineController.Spline = null;
    }

    public void RestoreSpline() {
        _splineController.Spline = _cachedSpline;
    }

    public void ResetMovablePosition() {
        // _cachedTransform.localPosition = new Vector3(_xOffset, 0, 0);
        _cachedTransform.localRotation = Quaternion.identity;
    }
}
