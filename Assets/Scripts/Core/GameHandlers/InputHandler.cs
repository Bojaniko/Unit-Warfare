using UnitWarfare.Core;

namespace UnitWarfare.Input
{
    public class InputHandler : GameHandler
    {
        private readonly InputData _data;
        public InputData Data => _data;

        private DefaultInput _input;

        protected override void Initialize()
        {
            _input.Enable();
        }

        // ##### POST PROCESSORS ##### \\

        private UserInterfaceInputTracker _uiTracker;
        public UserInterfaceInputTracker UI_InputTracker => _uiTracker;

        private event UserInterfaceInputTrackerEventHandler OnUIInteractionChanged;

        private PintchProcessor pp_pintch;
        public PintchProcessor PintchInput => pp_pintch;

        private MoveProcessor pp_move;
        public MoveProcessor MoveInput => pp_move;

        private TapProcessor pp_tap;
        public TapProcessor TapInput => pp_tap;

        private void InitPostProcessors()
        {
            _uiTracker = new(_input.UI.Click, _input.UI.Point);
            _uiTracker.OnUIInteractionChanged += (interaction) => { OnUIInteractionChanged?.Invoke(interaction); };

            PintchProcessor.Config pintchConfig = new(
                _input.Touch.PintchTouchOne,
                _input.Touch.PintchTouchTwo,
                _input.Touch.PintchPositionOne,
                _input.Touch.PintchPositionTwo, _data.PintchData);
            pp_pintch = new(pintchConfig, ref OnUIInteractionChanged);

            MoveProcessor.Config moveConfig = new(
                _input.Touch.MoveTouch,
                _input.Touch.Position,
                _data.MoveData);
            pp_move = new(moveConfig, ref OnUIInteractionChanged);

            TapProcessor.Config tapConfig = new(
                _input.Touch.Touch,
                _input.Touch.Position);
            pp_tap = new(tapConfig, ref OnUIInteractionChanged);
        }

        // ##### INSTANCE #####

        public InputHandler(InputData data, IGameStateHandler game_state_handler)
            : base(game_state_handler)
        {
            _data = data;

            _input = new();

            InitPostProcessors();
        }
    }
}