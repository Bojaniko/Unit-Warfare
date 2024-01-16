using UnityEngine;

namespace UnitWarfare.Cameras
{
    [CreateAssetMenu(menuName = "Camera")]
    public class CameraData : ScriptableObject
    {
        [Header("Distance")]

        [SerializeField] private float _distanceSpeed = 3f;
        public float DistanceSpeed => _distanceSpeed;

        [SerializeField, Range(1f, 20f)] private float _distance = 5f;
        public float StartDistance => _distance;

        [SerializeField] private float _distanceEase = 0.5f;
        public float DistanceEase => _distanceEase;

        [SerializeField] private Vector2 _distanceLimit = new Vector2(2f, 10f);
        public Vector2 DistanceLimit => _distanceLimit;

        [Header("Yaw")]

        [SerializeField] private float _yawSpeed = 3f;
        public float YawSpeed => _yawSpeed;

        [SerializeField, Range(0f, 1f)] private float _startYaw = 0.5f;
        public float StartYaw => _startYaw;

        [SerializeField, Range(0f, 1f)] private float _yawEase = 0.5f;
        public float YawEase => _yawEase;

        [Header("Pitch")]

        [SerializeField] private float _pitchSpeed = 3f;
        public float PitchSpeed => _pitchSpeed;

        [SerializeField, Range(0f, 1f)] private float _startPitch = 0.2f;
        public float StartPitch => _startPitch;

        [SerializeField, Range(0f, 1f)] private float _pitchEase = 0.5f;
        public float PitchEase => _pitchEase;

        [SerializeField] private Vector2 _pitchAngleLimits = new(20f, 80f);
        public Vector2 PitchAngleLimits => _pitchAngleLimits;
    }
}