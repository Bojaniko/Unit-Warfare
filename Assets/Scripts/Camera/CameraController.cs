using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Input;
using UnitWarfare.Territories;

using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Cameras
{
    public class CameraController
    {
        public record CameraConfig(Camera Camera, CameraData Data, Vector3 MapCenter, MoveProcessor MoveInput, PintchProcessor PintchInput);

        private readonly CameraConfig _config;

        public CameraData Data => _config.Data;
        public Camera Camera => _config.Camera;

        private EncapsulatedMonoBehaviour _emb;

        private readonly float _minPitch;
        private readonly float _maxPitch;

        private float _pitch;
        private float _pitchTarget;
        private Vector3 v3_pitch;

        private float _yaw;
        private float _yawTarget;
        private Vector3 v3_yaw;

        private float _distance;
        private float _distanceTarget;

        private bool _pintching;

        public CameraController(CameraConfig config)
        {
            _config = config;
            _emb = new(_config.Camera.gameObject);
            _emb.OnUpdate += Update;

            _config.MoveInput.OnInput += HandleMoveInput;
            _config.PintchInput.OnInput += HandlePintchInput;
            _config.PintchInput.OnPintchEnd += () =>
                _pintching = false;

            _pintching = false;

            _minPitch = _config.Data.PitchAngleLimits.x / 360f;
            _maxPitch = _config.Data.PitchAngleLimits.y / 360f;

            _distance = _config.Data.StartDistance;
            _distanceTarget = 0f;

            _pitch = _config.Data.StartPitch;
            _pitchTarget = 0f;

            _yaw = _config.Data.StartYaw;
            _yawTarget = 0f;

            UpdateYawVector();
            UpdatePitchVector();
            UpdateRotation();
        }

        private void HandlePintchInput(PintchProcessor.Output output)
        {
            if (!_pintching)
                _pintching = true;

            _distanceTarget = GetTarget(_distanceTarget, output.Value, 1f, _config.Data.DistanceSpeed);
        }

        private void HandleMoveInput(MoveProcessor.Output output)
        {
            if (_pintching)
                return;

            if (output.Axis.Equals(MoveProcessor.MoveAxis.HORIZONTAL))
                _yawTarget = GetTarget(_yawTarget, output.Direction, 1f, _config.Data.YawSpeed);
            else if (output.Axis.Equals(MoveProcessor.MoveAxis.VERTICAL))
                _pitchTarget = GetTarget(_pitchTarget, output.Direction, 1f, _config.Data.PitchSpeed);
        }

        private void Update()
        {
            if (_yawTarget == 0f && _pitchTarget == 0f && _distanceTarget == 0f)
                return;

            if (_yawTarget != 0f)
            {
                UpdateYaw();
                UpdateYawVector();
            }
            if (_pitchTarget != 0f)
            {
                UpdatePitch();
                UpdatePitchVector();
            }
            if (_distanceTarget != 0f)
                UpdateDistance();

            UpdateRotation();
        }

        private void UpdateYaw()
        {
            _yaw += -_yawTarget * _config.Data.YawSpeed * 0.1f * Time.deltaTime;

            _yawTarget = GetTarget(_yawTarget, 0f, _config.Data.YawEase, _config.Data.YawSpeed);

            if (_yaw > 1.0f)
                _yaw -= 1.0f;
            if (_yaw < 0.0f)
                _yaw = 1.0f - (-_yaw);
        }

        private void UpdateYawVector() =>
            v3_yaw = new Vector3(Mathf.Cos(_yaw * 360f * Mathf.Deg2Rad), 0f, Mathf.Sin(_yaw * 360f * Mathf.Deg2Rad));

        private void UpdatePitch()
        {
            _pitch += -_pitchTarget * _config.Data.PitchSpeed * 0.1f * Time.deltaTime;

            _pitchTarget = GetTarget(_pitchTarget, 0f, _config.Data.PitchEase, _config.Data.PitchSpeed);

            if (_pitch > 1.0f)
                _pitch = 1.0f;
            if (_pitch < 0.0f)
                _pitch = 0.0f;
        }

        private void UpdatePitchVector()
        {
            float p = _pitch * (_maxPitch - _minPitch) + _minPitch;
            v3_pitch = new Vector3(Mathf.Cos(p * 360f * Mathf.Deg2Rad), Mathf.Sin(p * 360f * Mathf.Deg2Rad), Mathf.Cos(p * 360f * Mathf.Deg2Rad));
        }

        private void UpdateDistance()
        {
            _distance += _distanceTarget * _config.Data.DistanceSpeed * Time.deltaTime;

            _distanceTarget = GetTarget(_distanceTarget, 0f, _config.Data.DistanceEase, _config.Data.DistanceSpeed);

            if (_distance > _config.Data.DistanceLimit.y)
                _distance = _config.Data.DistanceLimit.y;
            if (_distance < _config.Data.DistanceLimit.x)
                _distance = _config.Data.DistanceLimit.x;
        }
        
        private void UpdateRotation()
        {
            Vector3 yawpitch = new Vector3(v3_yaw.x * v3_pitch.x, v3_pitch.y, v3_yaw.z * v3_pitch.z);
            _emb.transform.position = _config.MapCenter + yawpitch * _distance;
            _emb.transform.LookAt(_config.MapCenter);
        }

        private float GetTarget(float current, float target, float ease, float speed)
        {
            if (current > target)
            {
                current -= ease * speed * Time.deltaTime;
                if (current < target)
                    current = target;
            }
            else if (current < target)
            {
                current += ease * speed * Time.deltaTime;
                if (current > target)
                    current = target;
            }

            return current;
        }

        public SelectionTarget GetTargetFromScreenPosition(Vector2 position)
        {
            Ray ray = Camera.ScreenPointToRay(position);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.parent == null)
                    return null;
                Territory targetedTerritory =
                    ((Territory.TerritoryEMB)
                    (hit.transform.parent.gameObject.GetComponent<Territory.TerritoryEMB.EMB>().Encapsulator)).Territory;
                if (targetedTerritory != null)
                    return new SelectionTarget(targetedTerritory, position);
            }
            return null;
        }
    }
}