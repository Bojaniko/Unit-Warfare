using UnityEngine;

using UnitWarfare.Input;

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnitWarfare.Test
{
    public class TapTester : MonoBehaviour
    {
        [SerializeField] private InputData _inputData;

        [SerializeField] private GameObject _testObject;

        private void Start()
        {
            TestGameStateHandler tg = TestGameStateHandler.CreateInstance();

            InputHandler input = tg.GetHandler<InputHandler>();
            if (input == null)
            {
                input = new(_inputData, tg);
                tg.RegisterGameHandler(input);
            }

            tg.SetGameState(Core.Enums.GameState.LOADING);

            input.TapInput.OnInput += (data) =>
            {
                TapProcessor.Output output = (TapProcessor.Output)data;

                Ray ray = Camera.main.ScreenPointToRay(output.Position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    _testObject.transform.position = hit.point;
            };
        }
    }
}