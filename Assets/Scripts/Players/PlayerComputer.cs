using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Units;
using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Players
{
    public class PlayerComputer : Player
    {
        private readonly AiBrainData _aiData;
        private readonly AiBrain _activeUnitBrain;
        private readonly IUnitsHandler _unitsHandler;

        private IUnit[] _currentUnits;

        public PlayerComputer(IUnitsHandler units_handler, AiBrainData ai_data, PlayerData data, ref ActivePlayerEventHandler active_player_handler)
            : base(data, ref active_player_handler)
        {
            _aiData = ai_data;

            _unitsHandler = units_handler;

            AiBrainFeature[] activeUnitFeatures = new[]
            {
                AiBrainFeature.AGRESSIVE,
                AiBrainFeature.PASSIVE,
                AiBrainFeature.CONQUERING,
                AiBrainFeature.TEAMPLAY,
                AiBrainFeature.COWARDICE
            };
            _activeUnitBrain = new AiBrain(ExtractBrainFeatures(_aiData.BrainFeatures, activeUnitFeatures), _aiData.ReductionFactor, _aiData.NormalizationStep);
        }

        private Coroutine _unitsRoutine;

        private IEnumerator UnitsRoutine()
        {
            foreach (IUnit unit in _currentUnits)
            {
                IActiveUnit activeUnit = unit as IActiveUnit;
                if (activeUnit != null)
                {
                    yield return ActiveUnitMoveHandler(activeUnit);
                    continue;
                }
                Debug.Log("Not active unit");
            }
        }

        private IEnumerator ActiveUnitMoveHandler(IActiveUnit unit)
        {
            bool enemyTerritoryAvailable = false;
            bool enemyOccupiedTerritoryAvailable = false;
            bool retreatableTerritoryAvailable = false;
            bool joinableAllyTerritoryAvailable = false;

            foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
            {
                
            }

            yield return null;
        }

        protected override void OnActiveTurn()
        {
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

        private BrainFeature[] ExtractBrainFeatures(BrainFeature[] features, AiBrainFeature[] extracted_features)
        {
            List<BrainFeature> extracted = new();
            foreach (BrainFeature bf in features)
            {
                foreach (AiBrainFeature f in extracted_features)
                {
                    if (bf.Feature.Equals(f))
                        extracted.Add(bf);
                }
            }
            return extracted.ToArray();
        }
    }
}