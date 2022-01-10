using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;

    [Header("Change Camera")]
    public Transform sedanTargetOut;
    public Transform sedanTargetInside;
    public CarController sedanController;

    public Transform lightTruckTargetOut;
    public Transform lightTruckTargetInside;
    public CarController lightTruckController;

    public SteeringWheelController steeringWheel;

    [Header("Position")]
    [SerializeField] private float _followSpeed = 18.0f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 1.5f;

    [Header("Cam Distance")]
    [SerializeField] private float _truckDistanceZ = 9.0f;
    [SerializeField] private float _truckDistanceY = 1.75f;
    [SerializeField] private float _camaroDistanceZ = 7.0f;
    [SerializeField] private float _camaroDistanceY = 1.5f;

    private Vector3 _followPosition;
    private Transform _cam;
    private bool _looksInside;

    private void Start()
    {
        _cam = transform.GetChild(0);
        SetCameraDistance();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            ChangeCamera();
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            ChangeTarget();
        }

        if(_looksInside) SetCamPosition();
        
    }

    // Must 2 setcamposition one is in the fixedUpdate other is update for fix jittering and lagging
    private void FixedUpdate()
    {
        SetCamRotation();
        if (!_looksInside) SetCamPosition();
    }

    public void ChangeTarget()
    {
        if (followTarget.Equals(lightTruckTargetOut))
        {
            steeringWheel.carController = sedanController;

            followTarget = sedanTargetOut;
            sedanController.enabled = true;
            lightTruckController.enabled = false;

            SetCameraDistance();
        }
        else
        {
            steeringWheel.carController = lightTruckController;

            followTarget = lightTruckTargetOut;
            lightTruckController.enabled = true;
            sedanController.enabled = false;

            SetCameraDistance();
        }

        _looksInside = true;
        ChangeCamera();
    }

    public void ChangeCamera()
    {
        if (_looksInside)
        {
            if (followTarget.Equals(lightTruckTargetInside))
            {
                followTarget = lightTruckTargetOut;
            }
            else if (followTarget.Equals(sedanTargetInside))
            {
                followTarget = sedanTargetOut;
            }

            SetCameraDistance();
            _looksInside = false;
        }
        else
        {
            if (followTarget.Equals(lightTruckTargetOut))
            {
                followTarget = lightTruckTargetInside;
            }
            else if (followTarget.Equals(sedanTargetOut))
            {
                followTarget = sedanTargetInside;
            }

            SetCameraDistance(true);
            _looksInside = true;
        }
    }

    private void SetCamPosition()
    {
        if(_looksInside)
        {
            transform.position = followTarget.position;
        }   
        else
        {
            _followPosition = Vector3.Lerp(_followPosition, followTarget.position, _followSpeed * Time.deltaTime);
            transform.position = _followPosition;
        }
    }

    private void SetCamRotation()
    {
        if (_looksInside)
        {
            transform.rotation = followTarget.rotation;
        }
        else
        {
            Quaternion newRot = Quaternion.Euler(0.0f, followTarget.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, _rotationSpeed * Time.deltaTime);
        }
    }

    private void SetCameraDistance(bool lookInside = false)
    {
        if (lookInside)
        {
            _cam.localPosition = Vector3.zero;
            return;
        }

        if (followTarget.Equals(lightTruckTargetOut))
        {
            _cam.localPosition = new Vector3(0.0f, _truckDistanceY, -_truckDistanceZ);
        }
        else
        {
            _cam.localPosition = new Vector3(0.0f, _camaroDistanceY, -_camaroDistanceZ);
        }
    }
}
