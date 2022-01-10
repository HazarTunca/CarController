using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMeshUpdater : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _driverFront;
    [SerializeField] private WheelCollider _passengerFront;
    [SerializeField] private WheelCollider _driverRear;
    [SerializeField] private WheelCollider _passengerRear;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform _driverFrontTransform;
    [SerializeField] private Transform _passengerFrontTransform;
    [SerializeField] private Transform _driverRearTransform;
    [SerializeField] private Transform _passengerRearTransform;

    private void Update()
    {
        WheelMeshUpdate(_driverFront, _driverFrontTransform);
        WheelMeshUpdate(_passengerFront, _passengerFrontTransform);
        WheelMeshUpdate(_driverRear, _driverRearTransform);
        WheelMeshUpdate(_passengerRear, _passengerRearTransform);
    }

    private void WheelMeshUpdate(WheelCollider wheelCol, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCol.GetWorldPose(out position, out rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
