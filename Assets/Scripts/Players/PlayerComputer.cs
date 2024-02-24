using System.Collections;
using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Units;
using UnitWarfare.Territories;

namespace UnitWarfare.Players
{
    public class PlayerComputer : Player
    {
        public record Config(AiBrainData AiData, IUnitsHandler UnitsHandler);

        private AiBrain _brain;

        private IUnit[] _currentUnits;

        private Config _config;

        public PlayerComputer(PlayerData data, IPlayersHandler handler)
            : base(data, handler)
        {
        }

        public void Configure(Config configuration)
        {
            if (_config != null)
                return;
            _config = configuration;

            AiBrain.Config aiConfig = new(_config.AiData.ReductionFactor, _config.AiData.IncreasionAmount, _config.AiData.NormalizationStep);
            _brain = new AiBrain(_config.AiData.BrainFeatures, aiConfig);
        }

        private Coroutine _unitsRoutine;

        public override event PlayerEventHandler OnExplicitMoveEnd;

        private IEnumerator UnitsRoutine()
        {
            foreach (IUnit unit in _currentUnits)
            {
                if (unit.IsDead)
                    continue;
                yield return new WaitUntil(() =>
                {
                    foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
                    {
                        if (!t.Interactable)
                            return false;
                    }
                    return true;
                });
                IUnitCommand[] commands = _config.UnitsHandler.InteractionsHandler.GenerateCommands(unit);
                IUnitCommand command = _brain.GetOutcome(unit, commands);
                if (command == null)
                    continue;
                unit.StartCommand(command);
                yield return new WaitUntil(() => { return !unit.IsDoingSomething; });
            }
            OnExplicitMoveEnd?.Invoke(this);
        }

        protected override void OnActiveTurn()
        {
            if (_config == null)
                throw new UnityException("Computer player must be configurated!");
            _currentUnits = _config.UnitsHandler.GetUnits(this);
            _unitsRoutine = emb.StartCoroutine(UnitsRoutine());
        }

        protected override void OnInactiveTurn()
        {
            if (_unitsRoutine != null)
            {
                emb.StopCoroutine(_unitsRoutine);
                _unitsRoutine = null;
            }
        }
    }
}