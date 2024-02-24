using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace UnitWarfare.Players
{


    public class PlayerLocalNetwork : PlayerLocal, IOnEventCallback
    {
        public PlayerLocalNetwork(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {
        }

        public void OnEvent(EventData photonEvent)
        {
            if (!photonEvent.Code.Equals(GlobalValues.NETWORK_UNIT_COMMAND_CODE))
                return;
        }

        protected override void StartUnitCommand(IUnit unit, IUnitCommand command)
        {
            //PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_UNIT_COMMAND_CODE, )
        }
    }

    public class PlayerLocal : Player
    {
        public record Config(TapProcessor TapInput, CameraController MainCamera, MatchProgress MatchProgress, UnitDisplay UnitDisplay, IUnitsHandler UnitsHandler);
        protected Config config;

        public PlayerLocal(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {

        }

        public void Configure(Config configuration)
        {
            if (config != null)
                return;
            config = configuration;
            config.TapInput.OnInput += HandleTapInput;
            config.MatchProgress.OnSkip += () => OnExplicitMoveEnd?.Invoke(this);
        }

        protected sealed override void OnActiveTurn()
        {
            if (config == null)
                throw new UnityException("Configuration not setup for local player.");
        }

        protected sealed override void OnInactiveTurn()
        {
            ClearSelection();
        }

        // ##### INPUT ##### \\

        private void HandleTapInput(TapProcessor.Output output)
        {
            if (!IsActive)
                return;
            HandleSelection(config.MainCamera.GetTargetFromScreenPosition(output.Position));
        }

        // ##### SELECTION ##### \\

        private SelectionTarget _selection;

        public sealed override event PlayerEventHandler OnExplicitMoveEnd;

        private void HandleSelection(SelectionTarget selection)
        {
            if (selection == null)
                return;
            if (_selection == null)
            {
                ActivateSelection(selection);
                return;
            }
            if (_selection.Territory.Equals(selection.Territory))
            {
                ClearSelection();
                return;
            }
            if (config.UnitsHandler.UnitExecutingCommand)
            {
                ActivateSelection(selection);
                return;
            }
            if (_selection.Unit != null && _selection.Territory.Owner.Equals(this))
            {
                UnitTarget target = new(selection.Territory);
                IUnitCommand command = config.UnitsHandler.InteractionsHandler.GenerateCommand(_selection.Unit, target);
                StartUnitCommand(_selection.Unit, command);
                if (_selection.Unit.IsDoingSomething)
                    ClearSelection();
                else
                    ActivateSelection(selection);
                return;
            }
            ActivateSelection(selection);
        }

        protected virtual void StartUnitCommand(IUnit unit, IUnitCommand command)
        {
            unit.StartCommand(command);
        }

        private void ActivateSelection(SelectionTarget selection)
        {
            ClearSelection();
            _selection = selection;

            _selection.Territory.EnableSelection(Territory.SelectionType.ACTIVE);

            IUnit au = _selection.Unit;
            if (au != null)
                config.UnitDisplay.DisplayUnit(new(au.Data.DisplayTexture, au.Data.DisplayName, au.Data.Attack, au.Data.Shield, au.Data.Health));
        }

        private void ClearSelection()
        {
            if (_selection == null)
                return;

            _selection.Territory.DisableSelection();
            config.UnitDisplay.HideDisplay();

            _selection = null;
        }
    }
}