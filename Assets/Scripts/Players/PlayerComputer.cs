using System.Collections;
using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Units;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

namespace UnitWarfare.Players
{
    public class PlayerComputer : Player
    {
        private readonly AiBrainData _aiData;
        private readonly AiBrain _brain;
        private readonly IUnitsHandler _unitsHandler;

        private IUnit[] _currentUnits;

        public PlayerComputer(IUnitsHandler units_handler, AiBrainData ai_data, PlayerData data, PlayerIdentification identification, IPlayersHandler handler)
            : base(data, identification, handler)
        {
            _aiData = ai_data;

            _unitsHandler = units_handler;

            AiBrain.Config config = new(ai_data.ReductionFactor, ai_data.IncreasionAmount, ai_data.NormalizationStep);
            _brain = new AiBrain(_aiData.BrainFeatures, config);
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
                IUnitCommand[] commands = _unitsHandler.InteractionsHandler.GenerateCommands(unit);
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
            Debug.Log("Computer player's turn.");
            _currentUnits = _unitsHandler.GetUnitsForOwner(this);
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