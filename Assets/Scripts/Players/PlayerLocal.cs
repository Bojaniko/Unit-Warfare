using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Players
{
    public class PlayerLocal : Player
    {
        public record Config(TapProcessor TapInput, CameraController MainCamera, MatchProgress MatchProgress, UnitDisplay UnitDisplay, IUnitsHandler UnitsHandler);
        private readonly Config _config;

        public PlayerLocal(Config config, PlayerData data, PlayerIdentification identification, IPlayerHandler handler)
            : base(data, identification, handler)
        {
            _config = config;

            _config.TapInput.OnInput += HandleTapInput;
            _config.MatchProgress.OnSkip += () => OnExplicitMoveEnd?.Invoke(this);
        }

        protected override void OnActiveTurn()
        {
            Debug.Log("Local player's turn.");
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
            HandleSelection(_config.MainCamera.GetTargetFromScreenPosition(output.Position));
        }

        // ##### SELECTION ##### \\

        private SelectionTarget _selection;

        public override event PlayerEventHandler OnExplicitMoveEnd;

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
            if (_config.UnitsHandler.UnitExecutingCommand)
            {
                ActivateSelection(selection);
                return;
            }
            if (_selection.Unit != null && _selection.Territory.Owner.Identification.Equals(Identification))
            {
                UnitTarget target = new(selection.Territory);
                IUnitCommand command = _config.UnitsHandler.InteractionsHandler.GenerateCommand(_selection.Unit, target);
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
        }

        private void ActivateSelection(SelectionTarget selection)
        {
            ClearSelection();
            _selection = selection;

            _selection.Territory.EnableSelection(Territory.SelectionType.ACTIVE);

            IUnit au = _selection.Unit;
            if (au != null)
                _config.UnitDisplay.DisplayUnit(new(au.Data.DisplayTexture, au.Data.DisplayName, au.Data.Attack, au.Data.Shield, au.Data.Health));
        }

        private void ClearSelection()
        {
            if (_selection == null)
                return;

            _selection.Territory.DisableSelection();
            _config.UnitDisplay.HideDisplay();

            _selection = null;
        }
    }
}