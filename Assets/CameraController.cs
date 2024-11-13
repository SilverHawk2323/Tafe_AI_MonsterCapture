using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    #region Focus Variables
    [Header("Focus")]
    [SerializeField] private Transform _focus;
    [SerializeField] private Vector3 _focusOffset;

    [SerializeField, Min(0f)] private float _focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float _focusCentering = 0.5f;
    private Vector3 _focusPoint, _previousFocusPoint;
    #endregion


    #region Oribit Variables
    private Vector2 _orbitAngles = new Vector2(45f, 0);
    [Header("Orbit")]
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private float _orbitDistance = 5f;
    #endregion

    private void Awake()
    {
        _focusPoint = _focus.position + _focusOffset;
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    //Camera controls should always be in late update
    private void LateUpdate()
    {
        Quaternion lookRotation = transform.localRotation;

        UpdateFocusPoint();

        if (ManualRotation() || AutomaticRotation())
        {
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;

        Vector3 lookPosition = _focusPoint - lookDirection * _orbitDistance;
        transform.SetPositionAndRotation(lookPosition,lookRotation);
    }

    private bool ManualRotation()
    {
        Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        float e = 0.001f; //Deadzone
        if(Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
        {
            _orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * input;
            return true;
        }

        return false;
    }

    private bool AutomaticRotation()
    {
        return false;
    }

    private void UpdateFocusPoint()
    {
        _previousFocusPoint = _focusPoint;
        Vector3 targetFocusPoint = _focus.position + _focusOffset;

        if (_focusRadius > 0f)
        {
            float moveDistance = Vector3.Distance(targetFocusPoint, _previousFocusPoint);
            float t = 1f;

            if(moveDistance > 0.01f && _focusCentering > 0f)
            {
                t = Mathf.Pow(1f - _focusCentering, Time.unscaledDeltaTime);
            }

            if(moveDistance > _focusRadius)
            {
                t = Mathf.Min(t, _focusRadius/moveDistance);
            }

            _focusPoint = Vector3.Lerp(targetFocusPoint, _previousFocusPoint, t);
        }
        else
        {
            _focusPoint = targetFocusPoint;
        }

        
    }
}
