using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Network;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

using ExitGames.Client.Photon;

namespace UnitWarfare.Players
{
    public class PlayerLocal : Player
    {
        public record Config(TapProcessor TapInput, CameraController MainCamera, MatchProgress MatchProgress, UnitDisplay UnitDisplay, IUnitsHandler UnitsHandler, bool IsNetwork);
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

        protected override void OnActiveTurn()
        {
            if (config == null)
                throw new UnityException("Configuration not setup for local player.");
        }

        protected override void OnInactiveTurn()
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

        public override event PlayerEventHandler OnExplicitMoveEnd;

        private void HandleSelection(SelectionTarget selection)
        {
            if (selection == null)
            {
                ClearSelection();
                return;
            }
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
            if (config.UnitsHandler.UnitsExecutingCommand.Length > 0)
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

        private void StartUnitCommand(IUnit unit, IUnitCommand command)
        {
            unit.StartCommand(command);
            if (config.IsNetwork)
            {
                NetworkUnitCommand nuc = new(unit, command);
                NetworkHandler.Instance.GameEvents.RaiseEvent(GlobalValues.NETWORK_UNIT_COMMAND_CODE, nuc, null, SendOptions.SendReliable);
            }
        }

        private void ActivateSelection(SelectionTarget selection)
        {
            ClearSelection();
            _selection = selection;

            config.MainCamera.SetTarget(_selection.Territory.EMB.transform);
            _selection.Territory.EnableSelection(Territory.SelectionType.ACTIVE);

            IUnit au = _selection.Unit;
            if (au != null)
                config.UnitDisplay.DisplayUnit(new(au.Data.DisplayTexture, au.Data.DisplayName, au.Data.Attack, au.Data.Shield, au.Data.Health));
        }

        private void ClearSelection()
        {
            if (_selection == null)
                return;

            config.MainCamera.SetTarget(null);
            _selection.Territory.DisableSelection();
            config.UnitDisplay.HideDisplay();

            _selection = null;
        }
    }
}