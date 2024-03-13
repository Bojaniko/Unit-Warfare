using EventSystem = UnityEngine.EventSystems.EventSystem;
using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Input
{
    public class InputHandler : GameHandler
    {
        private readonly InputData _data;
        public InputData Data => _data;

        private readonly DefaultInput _input;

        private UserInterfaceInputTracker ui_inputTracker;

        protected override void Initialize()
        {
            _input.Enable();
        }

        // ##### POST PROCESSORS ##### \\

        private PintchProcessor pp_pintch;
        public PintchProcessor PintchInput => pp_pintch;

        private MoveProcessor pp_move;
        public MoveProcessor MoveInput => pp_move;

        private TapProcessor pp_tap;
        public TapProcessor TapInput => pp_tap;

        private void InitPostProcessors()
        {
            PintchProcessor.Config pintchConfig = new(
                _input.Touch.PintchTouchOne,
                _input.Touch.PintchTouchTwo,
                _input.Touch.PintchPositionOne,
                _input.Touch.PintchPositionTwo, _data.PintchData);
            pp_pintch = new(pintchConfig);

            MoveProcessor.Config moveConfig = new(
                _input.Touch.MoveTouch,
                _input.Touch.Position,
                _data.MoveData);
            pp_move = new(moveConfig);

            TapProcessor.Config tapConfig = new(
                _input.Touch.Touch,
                _input.Touch.Position);
            pp_tap = new(tapConfig);
        }

        protected override void Enable()
        {
            pp_pintch.Enable();
            pp_move.Enable();
            pp_tap.Enable();
        }

        protected override void Disable()
        {
            pp_pintch.Disable();
            pp_move.Disable();
            pp_tap.Disable();
        }

        // ##### INSTANCE #####

        public InputHandler(InputData data, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _data = data;

            _input = new();

            EventSystem eventSystem = GameObject.Find(GlobalValues.GAME_EVENT_SYSTEM_NAME).GetComponent<EventSystem>();
            if (eventSystem == null)
                throw new UnityException($"InputHandler requires an event system present in the level with the name {GlobalValues.GAME_EVENT_SYSTEM_NAME}.");
            ui_inputTracker = new(_input.UI.Click, eventSystem);
            ui_inputTracker.OnUIInteraction += (interacting) =>
            {
                if (interacting)
                    Disable();
                else
                    Enable();
            };

            InitPostProcessors();
        }
    }
}