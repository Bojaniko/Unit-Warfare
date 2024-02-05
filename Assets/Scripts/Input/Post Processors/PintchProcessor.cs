using UnityEngine;
using UnityEngine.InputSystem;

using UnitWarfare.Core;

namespace UnitWarfare.Input
{
	public class PintchProcessor : InputPostProcessor<PintchProcessor.Output>
	{
		public record Config(InputAction TouchOne, InputAction TouchTwo,
			InputAction PositionOne, InputAction PositionTwo,
			PintchProcessorData Data);
		private readonly Config _config;

		private bool _touchOneActive;
		private bool _touchTwoActive;

		private float _previousDistance;

		private readonly Camera c_camera;

		public delegate void PintchEventHandler();
		public event PintchEventHandler OnPintchEnd;

		public PintchProcessor(Config config, ref UserInterfaceInputTrackerEventHandler ui_interaction)
			: base(ref ui_interaction)
		{
			_config = config;

			_touchOneActive = false;
			_touchTwoActive = false;

			BindInput(_config.TouchOne, _config.TouchTwo);

			c_camera = Camera.main;

			GameObject go = new("INPUT_POST_PROCESSOR: PINTCH_PROCESSOR");
			EncapsulatedMonoBehaviour emb = new(go);
			emb.OnUpdate += OnUpdate;
		}

		private void BindInput(InputAction touch_one, InputAction touch_two)
		{
			touch_one.performed += (ctx) =>
			{
				if (!_touchOneActive)
				{
					_touchOneActive = true;
					if (_touchTwoActive)
						_previousDistance = GetTouchesDistance();
					return;
				}
				if (_touchTwoActive)
					OnPintchEnd?.Invoke();
				_touchOneActive = false;
			};

			touch_two.performed += (ctx) =>
			{
				if (!_touchTwoActive)
				{
					_touchTwoActive = true;
					if (_touchOneActive)
						_previousDistance = GetTouchesDistance();
					return;
				}
				if (_touchOneActive)
					OnPintchEnd?.Invoke();
				_touchTwoActive = false;
			};
		}

		private float GetTouchesDistance()
		{
			Vector2 touchOneStartPosition = c_camera.ScreenToViewportPoint(_config.PositionOne.ReadValue<Vector2>());
			Vector2 touchTwoStartPosition = c_camera.ScreenToViewportPoint(_config.PositionTwo.ReadValue<Vector2>());
			return Vector2.Distance(touchOneStartPosition, touchTwoStartPosition);
		}

		// ##### DETECTION ##### \\

		private void OnUpdate()
		{
			if (!_touchOneActive || !_touchTwoActive)
				return;

			float currentDistance = GetTouchesDistance();

			if (Mathf.Abs(currentDistance - _previousDistance) < _config.Data.MinPintchTreshold)
				return;

			if (currentDistance > _previousDistance)
				SendInput(new Output(-1f));
			else if (currentDistance < _previousDistance)
				SendInput(new Output(1f));
			else
				SendInput(new Output(0f));

			_previousDistance = currentDistance;
		}

		// ##### OUTPUT ##### \\

		public sealed class Output : IInputPostProcessorOutput
		{
			private readonly float _value;
			public float Value => _value;

			internal Output(float @value)
            {
				_value = value;
            }
        }
	}
}