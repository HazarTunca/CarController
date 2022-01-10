using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public CarController carController;

    [SerializeField] private float maxAngle = 480f;
    [SerializeField] private float releaseSpeed = 450f;

    private float _wheelAngle = 0f;
    private float _wheelPrevAngle = 0f;
    private Vector2 _centerPoint;
    private RectTransform _wheel;
    private bool _startsDrag;

    private void Start()
    {
        _wheel = GetComponent<RectTransform>();
    }

    private void ApplyInput()
    {
        float divider = maxAngle / carController.maxTurnAngle;
        float wheelTurnAngle = _wheelAngle / divider;

        carController.currentTurnAngle = wheelTurnAngle;
    }

#region Events
    public void OnPointerDown(PointerEventData eventData)
    {
        _startsDrag = true;
        StartCalculatingWheelRotation(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CalculateWheelRotation(eventData);
        UpdateWheelImage();
        ApplyInput();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _startsDrag = false;
        OnDrag(eventData);
        StartCoroutine(ReleaseWheel());
    }
#endregion

#region Calculations
    private void StartCalculatingWheelRotation(PointerEventData eventData)
    {
        _centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, _wheel.position);
        _wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - _centerPoint);
    }

    private void CalculateWheelRotation(PointerEventData eventData)
    {
        Vector2 pointerPos = eventData.position;

        float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - _centerPoint);

        // Do nothing if the pointer is too close to the center of the wheel
        if ((pointerPos - _centerPoint).sqrMagnitude >= 400f)
        {
            if (pointerPos.x > _centerPoint.x)
                _wheelAngle += wheelNewAngle - _wheelPrevAngle;

            else
                _wheelAngle -= wheelNewAngle - _wheelPrevAngle;
        }

        // Make sure wheel angle never exceeds maximumSteeringAngle
        _wheelAngle = Mathf.Clamp(_wheelAngle, -maxAngle, maxAngle);
        _wheelPrevAngle = wheelNewAngle;
    }

    private IEnumerator ReleaseWheel()
    {
        while (_wheelAngle != 0f)
        {
            if (_startsDrag) break;

            float deltaAngle = releaseSpeed * Time.deltaTime;

            if (Mathf.Abs(deltaAngle) > Mathf.Abs(_wheelAngle))
                _wheelAngle = 0f;

            else if (_wheelAngle > 0f)
                _wheelAngle -= deltaAngle;

            else
                _wheelAngle += deltaAngle;


            UpdateWheelImage();

            ApplyInput();

            yield return null;
        }
    }

    private void UpdateWheelImage()
    {
        _wheel.localEulerAngles = new Vector3(0f, 0f, -_wheelAngle);
    }
#endregion
}
