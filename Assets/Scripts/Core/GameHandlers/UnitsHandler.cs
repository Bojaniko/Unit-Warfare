using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Players;
using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Units
{
    public class UnitsHandler : GameHandler, IUnitsHandler
    {
        private Transform c_units;

        private List<IUnit> _units;

        private readonly UnitsData _data;

        private PlayersHandler h_players;

        // ##### INITIALIZATION ##### \\

        public UnitsHandler(UnitsData data, IGameStateHandler handler) : base(handler)
        {
            _data = data;
            _interactions = new(new(_data.Combinations.Combinations));

            c_units = GameObject.Find("UNITS").transform;

            if (c_units == null)
                throw new UnityException("Game requires 'UNITS' GameObject in the scene to store all Units.");
        }

        protected override void Initialize()
        {
            h_players = gameStateHandler.GetHandler<PlayersHandler>();

            InitUnits();
        }

        protected override void OnFinalLoad()
        {
            RemoveIdentifiers();
        }

        private void InitUnits()
        {
            Transform units = GameObject.Find("UNITS").transform;
            if (units == null)
                throw new UnityException("There is no 'UNITS' game object found in the scene," +
                    "please use the MapCreator tool for placings units in the level!");

            _units = new();
            _identifiers = new();
            TerritoryManager t_manager = gameStateHandler.GetHandler<TerritoryManager>();

            foreach (Transform unit in units)
            {
                UnitIdentifier i = unit.GetComponent<UnitIdentifier>();
                if (i == null)
                    continue;
                IUnitTeamManager unitManager = h_players.GetPlayer(i.StartingTerritory.Owner);

                IUnit u = UnitFactory.GenerateUnit(t_manager.GetByIdentifier(i.StartingTerritory), i.Data, units, unitManager);
                _units.Add(u);
                _identifiers.Add(i, u);

                if (u is IActiveUnit)
                    InitActiveUnit(u as IActiveUnit);
            }
        }

        // ##### INTERFACE ##### \\

        /// <summary>
        /// Order of n
        /// </summary>
        public IUnit[] GetUnitsForOwner(ITerritoryOwner owner)
        {
            List<IUnit> units = new();
            foreach (IUnit unit in _units)
            {
                if (unit.OccupiedTerritory.Owner.Equals(owner))
                    units.Add(unit);
            }
            return units.ToArray();
        }

        public IUnit[] GetUnitsForOwnerType(PlayerIdentification owner)
        {
            List<IUnit> units = new();
            foreach (IUnit unit in _units)
            {
                if (unit.Owner.Equals(owner))
                    units.Add(unit);
            }
            return units.ToArray();
        }

        private readonly UnitInteractions _interactions;
        public UnitInteractions InteractionsHandler => _interactions;

        // ##### ACTIVE UNITS ##### \\

        private void InitActiveUnit(IActiveUnit unit)
        {
            unit.OnAttack += HandleActiveUnitAttack;
            unit.OnMove += HandleActiveUnitMove;
            unit.OnJoin += HandleActiveUnitJoin;
            (unit as IUnit).OnDestroy += HandleUnitDestroy;
        }

        private void HandleActiveUnitAttack(IActiveUnit attacking_unit, IUnit target_unit)
        {
            target_unit.Damage(((IUnit)attacking_unit).Attack);
        }

        private void HandleActiveUnitMove(IActiveUnit moving_unit, Territory target_territory)
        {
            ((IUnit)moving_unit).OccupiedTerritory.Deocuppy();
            ((IUnit)moving_unit).SetOccupiedTerritory(target_territory);
            target_territory.Occupy((ITerritoryOccupant)moving_unit);
        }

        private void HandleActiveUnitJoin(IActiveUnit joining_unit, IActiveUnit target_unit)
        {
            System.Type result = _interactions.CombinationsManager.GetResult(joining_unit.GetType(), target_unit.GetType());
            Territory territory = target_unit.OccupiedTerritory;
            joining_unit.DestroyUnit();
            target_unit.DestroyUnit();
            CreateActiveUnit(result, territory);
        }

        private void CreateActiveUnit(System.Type unit_type, Territory territory)
        {
            if (!unit_type.IsAssignableFrom(typeof(IActiveUnit)))
                return;

            UnitData data = null;

            foreach (PropertyInfo pi in typeof(UnitsData).GetProperties())
            {
                if (pi.PropertyType.Equals(unit_type.GetGenericArguments()[0]))
                    data = (UnitData)pi.GetValue(_data);
            }

            if (data == null)
                return;

            IUnitTeamManager unitManager = h_players.GetPlayer(territory.Owner.OwnerIdentification);
            IUnit new_unit = UnitFactory.GenerateUnit(territory, data, c_units, unitManager);
            _units.Add(new_unit);
            InitActiveUnit((IActiveUnit)new_unit);
        }

        // ##### UNITS ##### \\

        private void HandleUnitDestroy(IUnit unit)
        {
            unit.OnDestroy -= HandleUnitDestroy;

            IActiveUnit au = unit as IActiveUnit;
            if (au != null)
            {
                au.OnAttack -= HandleActiveUnitAttack;
                au.OnMove -= HandleActiveUnitMove;
                au.OnJoin -= HandleActiveUnitJoin;
            }

            _units.Remove(unit);
        }

        // ##### IDENTIFIERS ##### \\
        private Dictionary<UnitIdentifier, IUnit> _identifiers;

        private void RemoveIdentifiers()
        {
            foreach (UnitIdentifier i in _identifiers.Keys)
                Object.Destroy(i.gameObject);
            _identifiers = null;
        }

        public IUnit GetByIdentifier(UnitIdentifier identifier)
        {
            if (_identifiers == null)
                return null;
            return _identifiers[identifier];
        }
    }
}