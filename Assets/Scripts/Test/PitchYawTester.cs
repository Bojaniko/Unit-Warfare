using UnityEngine;

namespace UnitWarfare.Test
{
    [ExecuteInEditMode]
    public class PitchYawTester : MonoBehaviour
    {
        [SerializeField] private Transform _center;

        [SerializeField] private Transform _translatedObject;

        [SerializeField, Range(0f, 1f)] private float _pitch;
        [SerializeField, Range(0f, 1f)] private float _yaw;
        [SerializeField] private float _distance;

        [SerializeField] private Vector2 _anglesLimit;

        private void Update()
        {
            if (_center == null || _translatedObject == null)
                return;

            // Yaw is a point on a circle that is flat on the y axis
            Vector3 yaw = new Vector3(Mathf.Cos(_yaw * 360f * Mathf.Deg2Rad), 0f, Mathf.Sin(_yaw * 360f * Mathf.Deg2Rad));

            // P is the pitch remaped between two angles which range from 0 to 90 degrees
            float p = (_pitch * ((_anglesLimit.y / 360f) - (_anglesLimit.x / 360f))) + _anglesLimit.x / 360f;

            // Pitch is the height on a circle, with x and z coordinates to compensate for the direction
            Vector3 pitch = new Vector3(Mathf.Cos(p * 360f * Mathf.Deg2Rad), Mathf.Sin(p * 360f * Mathf.Deg2Rad), Mathf.Cos(p * 360f * Mathf.Deg2Rad));

            // The Yaw x and z components are used for determining the yaw rotation,
            // and are multiplied by the Pitch x and z components to keep the point on the sphere,
            // and the y component is the pitch height
            Vector3 yawpitch = new Vector3(yaw.x * pitch.x, pitch.y, yaw.z * pitch.z);

            _translatedObject.position = _center.transform.position + yawpitch * _distance;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_center.position, _distance);
        }
    }
}