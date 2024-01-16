using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Input;

namespace UnitWarfare.Test
{
    public class PintchTester : MonoBehaviour
    {
        [SerializeField] private InputData _inputData;

        [SerializeField] private GameObject _testObject;

        [SerializeField] private float _sensitivity = 0.5f;

        private void Start()
        {
            TestGameStateHandler tg = TestGameStateHandler.CreateInstance();

            InputHandler input = tg.GetHandler<InputHandler>();
            if (input == null)
            {
                input = new(_inputData, tg);
                tg.RegisterGameHandler(input);
            }

            input.PintchInput.OnInput += (data) =>
            {
                PintchProcessor.Output output = (PintchProcessor.Output)data;

                if (_testObject.Equals(null))
                    return;

                if (output.Value > 0f)
                {
                    _testObject.transform.localScale += Vector3.one * (_sensitivity * Time.deltaTime);
                    if (_testObject.transform.localScale.x > 2f)
                        _testObject.transform.localScale = Vector3.one * 2f;
                }
                if (output.Value < 0f)
                {
                    _testObject.transform.localScale -= Vector3.one * (_sensitivity * Time.deltaTime);
                    if (_testObject.transform.localScale.x < 0.5f)
                        _testObject.transform.localScale = Vector3.one * 0.5f;
                }
            };
        }
    }
}