using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour 
{
    [Flags] public enum Axis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }
    
    private Transform _mainCamera;
    private Transform _myTransform;
    private Axis _ignoredAxis = Axis.None;
    private Axis _useCameraAxis = Axis.None;
    
    private void Awake() 
    {
        _mainCamera = Camera.main.transform;
        _myTransform = transform;
    }

    private void LateUpdate() 
    {
        var rotation = Quaternion.LookRotation(_myTransform.position - _mainCamera.position);
        var euler = rotation.eulerAngles;
        ApplyCustomRotation();
        _myTransform.rotation = Quaternion.Euler(euler);
        
        void ApplyCustomRotation()
        {
            euler.x = _ignoredAxis.HasFlag(Axis.X) ? 0 : _useCameraAxis.HasFlag(Axis.X) ? _mainCamera.eulerAngles.x : euler.x;
            euler.y = _ignoredAxis.HasFlag(Axis.Y) ? 0 : _useCameraAxis.HasFlag(Axis.Y) ? _mainCamera.eulerAngles.y : euler.y;
            euler.z = _ignoredAxis.HasFlag(Axis.Z) ? 0 : _useCameraAxis.HasFlag(Axis.Z) ? _mainCamera.eulerAngles.z : euler.z;
        }
    }
    
    public void SetCamera(Transform currentCamera) 
    {
        _mainCamera = currentCamera;
    }
    
    public void SetIgnoredAxis(Axis axis) 
    {
        _ignoredAxis = axis;
    }
    
    public void UseCameraAxis(Axis axis)
    {
        _useCameraAxis = axis;
    }
}
