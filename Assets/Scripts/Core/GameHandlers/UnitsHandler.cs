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

                InitUnit(u);
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

        public bool UnitExecutingCommand => _unitsExecutingCommands.Count > 0;
        public bool HasMovableUnits(PlayerIdentification owner)
        {
            foreach (IUnit unit in _units)
            {
                if (unit.Owner.Equals(owner) && unit.MoveAvailable)
                    return true;
            }
            return false;
        }

        // ##### ACTIVE UNITS ##### \\

        private void InitActiveUnit(IActiveUnit unit)
        {
            if (unit == null)
                return;
            unit.OnAttack += HandleActiveUnitAttack;
            unit.OnMove += HandleActiveUnitMove;
            unit.OnJoin += HandleActiveUnitJoin;
        }

        private void HandleActiveUnitDestroyed(IActiveUnit unit)
        {
            if (unit == null)
                return;
            unit.OnAttack -= HandleActiveUnitAttack;
            unit.OnMove -= HandleActiveUnitMove;
            unit.OnJoin -= HandleActiveUnitJoin;
        }

        private void HandleActiveUnitAttack(IActiveUnit unit, UnitTarget target)
        {
            target.Unit.Damage(unit.Attack);
        }

        private void HandleActiveUnitMove(IActiveUnit unit, UnitTarget target)
        {
            unit.OccupiedTerritory.Deocuppy();
            unit.SetOccupiedTerritory(target.Territory);
            target.Territory.Occupy(unit);
        }

        private void HandleActiveUnitJoin(IActiveUnit unit, UnitTarget target)
        {
            System.Type result = _interactions.CombinationsManager.GetResult(unit.GetType(), target.Unit.GetType());
            Territory territory = target.Territory;
            unit.DestroyUnit();
            target.Unit.DestroyUnit();
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

        private bool HandleActiveUnitCommandStart(IActiveUnit unit, UnitCommand<ActiveCommandOrder> command)
        {
            if (unit == null || command == null)
                return false;
            if (command.Order.Equals(ActiveCommandOrder.MOVE))
                command.Target.Territory.SetInteractable(false);
            return true;
        }

        private bool HandleActiveUnitCommandEnd(IActiveUnit unit, UnitCommand<ActiveCommandOrder> command)
        {
            if (unit == null || command == null)
                return false;
            if (command.Order.Equals(ActiveCommandOrder.MOVE))
                command.Target.Territory.SetInteractable(true);
            return true;
        }

        // ##### UNITS ##### \\

        private List<IUnit> _unitsExecutingCommands;

        private void InitUnit(IUnit unit)
        {
            _unitsExecutingCommands = new();

            unit.OnDestroy += HandleUnitDestroy;
            unit.OnCommandStart += HandleUnitCommandStart;
            unit.OnCommandEnd += HandleUnitCommandEnd;

            InitActiveUnit(unit as IActiveUnit);
        }

        private void HandleUnitDestroy(IUnit unit)
        {
            _unitsExecutingCommands.Remove(unit);

            unit.OnDestroy -= HandleUnitDestroy;
            unit.OnCommandStart -= HandleUnitCommandStart;
            unit.OnCommandEnd -= HandleUnitCommandEnd;

            unit.OccupiedTerritory.Deocuppy();

            HandleActiveUnitDestroyed(unit as IActiveUnit);

            _units.Remove(unit);
        }

        private void HandleUnitCommandStart(IUnit unit, IUnitCommand command)
        {
            _unitsExecutingCommands.Add(unit);

            if (HandleActiveUnitCommandStart(unit as IActiveUnit, command as UnitCommand<ActiveCommandOrder>))
                return;
        }

        private void HandleUnitCommandEnd(IUnit unit, IUnitCommand command)
        {
            _unitsExecutingCommands.Remove(unit);

            if (HandleActiveUnitCommandEnd(unit as IActiveUnit, command as UnitCommand<ActiveCommandOrder>))
                return;
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