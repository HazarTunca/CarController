using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WheelMeshUpdater))]
public class CarController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text gearText;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _driverFront;
    [SerializeField] private WheelCollider _passengerFront;
    [SerializeField] private WheelCollider _driverRear;
    [SerializeField] private WheelCollider _passengerRear;

    [Header("Car Controls")] [Space(10)]
    public float maxTurnAngle = 30.0f;
    [SerializeField] private float _acceleration = 500.0f;
    [SerializeField] private float _breakForce = 300.0f;

    [HideInInspector] public float currentTurnAngle = 0.0f;

    private Rigidbody _rb;
    private float _currentAcceleration = 0.0f;
    private float _currentBreakForce = 0.0f;
    private bool _willGoForward = true;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void OnEnable() => SetGearOnBegin();

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        DebugControl();
#endif
        Move();
    }

    private void Move()
    {
        // apply acceleration => All wheels
        _driverRear.motorTorque = _currentAcceleration;
        _passengerRear.motorTorque = _currentAcceleration;
        _driverFront.motorTorque = _currentAcceleration;
        _passengerFront.motorTorque = _currentAcceleration;

        // break force => all wheels
        _driverFront.brakeTorque = _currentBreakForce;
        _passengerFront.brakeTorque = _currentBreakForce;
        _driverRear.brakeTorque = _currentBreakForce;
        _passengerRear.brakeTorque = _currentBreakForce;

        // turn the wheels
        // _currentTurnAngle = _maxTurnAngle * Input.GetAxis("Horizontal");
        _driverFront.steerAngle = currentTurnAngle;
        _passengerFront.steerAngle = currentTurnAngle;
    }

    // debug controls for editor
    private void DebugControl()
    {   
        // break
        if (Input.GetKey(KeyCode.Space)) _currentBreakForce = _breakForce;
        else _currentBreakForce = 0.0f;

        // acceleration
        bool _isMoveForward = false;
        if (Input.GetKey(KeyCode.W)) _isMoveForward = true;

        bool _isMoveBack = false;
        if (Input.GetKey(KeyCode.S)) _isMoveBack = true;

        if (_isMoveForward) _currentAcceleration = _acceleration;
        if (_isMoveBack) _currentAcceleration = -_acceleration;
        if (!_isMoveForward && !_isMoveBack) _currentAcceleration = 0.0f;

        // turn wheels
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
    }

#region UI button functions for mobile
    public void AcceleratePressed()
    {
        if (_willGoForward) _currentAcceleration = _acceleration;
        else _currentAcceleration = -_acceleration;
    }

    public void AccelerateUnpressed()
    {
        _currentAcceleration = 0.0f;
    }

    public void BreakPressed()
    {
        _currentBreakForce = _breakForce;
    }

    public void BreakUnpressed()
    {
        _currentBreakForce = 0.0f;
    }

    public void ChangeGear()
    {
        if (!enabled) return;
        if (_rb.velocity.magnitude > 10.0f) return;

        if (_willGoForward)
        {
            _willGoForward = false;
            gearText.text = "R";
            return;
        }

        _willGoForward = true;
        gearText.text = "D";
    }

    private void SetGearOnBegin()
    {
        _willGoForward = true;
        gearText.text = "D";
    }
#endregion
}
