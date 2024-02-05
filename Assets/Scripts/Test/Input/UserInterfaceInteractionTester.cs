using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using UnitWarfare.Input;
using UnitWarfare.UI;

namespace UnitWarfare.Test
{
    public class UserInterfaceInteractionTester : MonoBehaviour
    {
        [SerializeField] private EventSystem _eventSystem;

        [SerializeField] private InputData _inputData;

        private void Start()
        {
            TestGameStateHandler teg = TestGameStateHandler.CreateInstance();

            teg.RegisterGameHandler(new InputHandler(_inputData, teg));
            //teg.RegisterGameHandler(new UIHandler(teg));

            teg.SetGameState(Core.Enums.GameState.LOADING);

            teg.GetHandler<InputHandler>().TapInput.OnInput += (output) => { Debug.Log("Not tapping UI!"); };
        }
    }
}