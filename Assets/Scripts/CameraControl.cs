using System.ComponentModel.Design;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform _occlusionPivot;

    [SerializeField] private float _rotationVelocityFactor;
    [SerializeField] private float _maxPitchUpAngle;
    [SerializeField] private float _minPitchDownAngle;
    [SerializeField] private float _resetYawSpeed;
    [SerializeField] private float _zoomAccelerationFactor;
    [SerializeField] private float _zoomDeceleration;
    [SerializeField] private float _zoomMinDistance;
    [SerializeField] private float _zoomMaxDistance;
    [SerializeField] private float _deocclusionThreshold;
    [SerializeField] private float _deocclusionVelocity;

    private Transform   _cameraTransform;
    private float       _zoomAcceleration;
    private float       _zoomVelocity;
    private float       _zoomPosition;
    private Vector3     _deocclusionVector;

    private void Start()
    {
        _cameraTransform    = GetComponentInChildren<Camera>().transform;
        _zoomVelocity       = 0f;
        _zoomPosition       = _cameraTransform.localPosition.z;
        _deocclusionVector  = new Vector3(0, 0, _deocclusionThreshold);
    }

    private void Update()
    {
        //UpdatePitch();
        UpdateYaw();
        UpdateZoom();

        PreventOcclusion();
    }

    private void UpdatePitch()
    {
        Vector3 rotation = transform.localEulerAngles;

        rotation.x -= Input.GetAxis("Mouse Y") * _rotationVelocityFactor;

        if (rotation.x < 180f)
            rotation.x = Mathf.Min(rotation.x, _maxPitchUpAngle);
        else
            rotation.x = Mathf.Max(rotation.x, _minPitchDownAngle);

        transform.localEulerAngles = rotation;
    }

    private void UpdateYaw()
    {
        if (Input.GetButton("Camera"))
        {
            Vector3 rotation = transform.localEulerAngles;

            rotation.y += Input.GetAxis("Mouse X") * _rotationVelocityFactor;

            transform.localEulerAngles = rotation;
        }
        else
            ResetYaw();
    }

    private void ResetYaw()
    {
        Vector3 rotation = transform.localEulerAngles;

        if (rotation.y != 0f)
        {
            if (rotation.y < 180f)
                rotation.y = Mathf.Max(rotation.y - Time.deltaTime * _resetYawSpeed, 0f);
            else
                rotation.y = Mathf.Min(rotation.y + Time.deltaTime * _resetYawSpeed, 360f);

            transform.localEulerAngles = rotation;
        }
    }

    private void UpdateZoom()
    {
        UpdateZoomAcceleration();
        UpdateZoomVelocity();
        UpdateZoomPosition();
    }

    private void UpdateZoomAcceleration()
    {
        _zoomAcceleration = Input.GetAxis("Zoom") * _zoomAccelerationFactor;
    }

    private void UpdateZoomVelocity()
    {
        if (_zoomAcceleration != 0f)
            _zoomVelocity += _zoomAcceleration * Time.deltaTime;
        else if (_zoomVelocity > 0f)
        {
            _zoomVelocity -= _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Max(_zoomVelocity, 0f);
        }
        else
        {
            _zoomVelocity += _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Min(_zoomVelocity, 0f);
        }
    }

    private void UpdateZoomPosition()
    {
        if (_zoomVelocity != 0f)
        {
            Vector3 position = _cameraTransform.localPosition;

            position.z += _zoomVelocity * Time.deltaTime;

            if (position.z < -_zoomMaxDistance || position.z > -_zoomMinDistance)
            {
                position.z = Mathf.Clamp(position.z, -_zoomMaxDistance, -_zoomMinDistance);
                _zoomVelocity = 0f;
            }

            _cameraTransform.localPosition = position;
            _zoomPosition = position.z;
        }
    }

    private void PreventOcclusion()
    {
        Debug.DrawLine(_occlusionPivot.position,
            _cameraTransform.position - _cameraTransform.TransformDirection(_deocclusionVector));

        if (Physics.Linecast(_occlusionPivot.position,
            _cameraTransform.position - _cameraTransform.TransformDirection(_deocclusionVector),
            out RaycastHit hitInfo))
        {
            if (hitInfo.collider.CompareTag("WorldBoundary"))
            {
                _cameraTransform.position = hitInfo.point + _cameraTransform.TransformDirection(_deocclusionVector);
            }
            else
            {
                Vector3 position = _cameraTransform.localPosition;
                position.z += _deocclusionVelocity * Time.deltaTime;
                _cameraTransform.localPosition = position;
            }
        }
        else
            RevertDeocclusion();
    }

    private void RevertDeocclusion()
    {
        Vector3 localPosition = _cameraTransform.localPosition;

        if (localPosition.z > _zoomPosition)
        {
            localPosition.z = Mathf.Max(localPosition.z - _deocclusionVelocity * Time.deltaTime, _zoomPosition);

            Vector3 worldPosition = transform.TransformPoint(localPosition);

            if (!Physics.Linecast(_occlusionPivot.position,
                worldPosition - _cameraTransform.TransformDirection(_deocclusionVector)))
                _cameraTransform.localPosition = localPosition;
        }
    }

}
