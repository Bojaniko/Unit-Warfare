using System.Collections.Generic;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Players;
using UnitWarfare.Network;
using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Units
{
    public class UnitsHandler : GameHandler, IUnitsHandler
    {
        private List<IUnit> _units;

        private PlayersHandler handler_players;

        private readonly Transform m_unitsContainer;
        public Transform UnitContainer => m_unitsContainer;

        public UnitData GetUnitDataByUnit(IUnitOwner owner, System.Type unit_type)
        {
            if (handler_players.LocalPlayer.Equals(owner))
                return handler_players.LocalPlayer.UnitsData.GetDataByUnit(unit_type);
            else if (handler_players.OtherPlayer.Equals(owner))
                return handler_players.OtherPlayer.UnitsData.GetDataByUnit(unit_type);
            return null;
        }

        public UnitData GetUnitData(IUnitOwner owner, System.Type data_type)
        {
            if (handler_players.LocalPlayer.Equals(owner))
                return handler_players.LocalPlayer.UnitsData.GetData(data_type);
            else if (handler_players.OtherPlayer.Equals(owner))
                return handler_players.OtherPlayer.UnitsData.GetData(data_type);
            return null;
        }

        // ##### INITIALIZATION ##### \\

        private UnitInitializerMediator _initializer;

        public UnitsHandler(UnitCombinations combinations, IGameStateHandler handler) : base(handler)
        {
            _interactions = new(new(combinations.Combinations));
            _unitsExecutingCommand = new();

            m_unitsContainer = GameObject.Find(GlobalValues.MAP_UNITS_CONTAINER).transform;
            if (m_unitsContainer == null)
                throw new UnityException($"Game requires '{GlobalValues.MAP_UNITS_CONTAINER}' GameObject in the scene to store all Units.");
        }

        protected override void Initialize()
        {
            _initializer = new(this);

            handler_players = gameStateHandler.GetHandler<PlayersHandler>();

            InitUnits();
        }

        protected override void OnFinalLoad()
        {
            RemoveIdentifiers();
        }

        private void InitUnits()
        {
            _units = new();
            _identifiers = new();
            TerritoryManager t_manager = gameStateHandler.GetHandler<TerritoryManager>();

            foreach (Transform unit in m_unitsContainer)
            {
                UnitIdentifier i = unit.GetComponent<UnitIdentifier>();
                if (i == null)
                    continue;

                IUnitOwner unitManager = GetOwnerForInstantiation(i);

                IUnit u = UnitFactory.GenerateUnit(t_manager.GetByIdentifier(i.StartingTerritory), i.Data, m_unitsContainer, unitManager);
                _identifiers.Add(i, u);
                InitUnit(u);
            }
        }

        private IUnitOwner GetOwnerForInstantiation(UnitIdentifier identifier)
        {
            IUnitOwner owner;
            if (gameStateHandler.GameType.Equals(GameType.NETWORK))
            {
                if (NetworkHandler.Instance.IsHost)
                {
                    switch (identifier.StartingTerritory.Owner)
                    {
                        case PlayerIdentifiers.PLAYER_ONE:
                            owner = handler_players.LocalPlayer;
                            break;
                        case PlayerIdentifiers.PLAYER_TWO:
                            owner = handler_players.OtherPlayer;
                            break;
                        default:
                            owner = handler_players.NeutralPlayer;
                            break;
                    }
                }
                else
                {
                    switch (identifier.StartingTerritory.Owner)
                    {
                        case PlayerIdentifiers.PLAYER_ONE:
                            owner = handler_players.OtherPlayer;
                            break;
                        case PlayerIdentifiers.PLAYER_TWO:
                            owner = handler_players.LocalPlayer;
                            break;
                        default:
                            owner = handler_players.NeutralPlayer;
                            break;
                    }
                }
            }
            else
            {
                switch (identifier.StartingTerritory.Owner)
                {
                    case PlayerIdentifiers.PLAYER_ONE:
                        owner = handler_players.LocalPlayer;
                        break;
                    case PlayerIdentifiers.PLAYER_TWO:
                        owner = handler_players.OtherPlayer;
                        break;
                    default:
                        owner = handler_players.NeutralPlayer;
                        break;
                }
            }
            return owner;
        }

        // ##### INTERFACE ##### \\

        /// <summary>
        /// Order of n
        /// </summary>

        public IUnit[] GetUnits(IUnitOwner owner)
        {
            List<IUnit> units = new();
            foreach (IUnit unit in _units)
            {
                if (unit.IsDead)
                    continue;
                if (unit.Owner.Equals(owner))
                    units.Add(unit);
            }
            return units.ToArray();
        }

        private readonly UnitInteractions _interactions;
        public UnitInteractions InteractionsHandler => _interactions;

        public bool HasMovableUnits(IUnitOwner owner)
        {
            if (!owner.IsActive)
                return false;
            foreach (IUnit unit in _units)
            {
                if (unit.Owner.Equals(owner) && unit.MoveAvailable)
                    return true;
            }
            return false;
        }

        // ##### UNITS ##### \\

        public event System.Action<IUnit> OnUnitSpawned;
        public event System.Action<IUnit> OnUnitDespawned;

        private readonly List<IUnit> _unitsExecutingCommand;
        public IUnit[] UnitsExecutingCommand => _unitsExecutingCommand.ToArray();

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

        private void InitUnit(IUnit unit)
        {
            _initializer.InitUnit(unit);
            _units.Add(unit);

            unit.OnDestroy += RemoveUnitFromList;
            unit.OnCommandStart += LogUnitCommandStarted;
            unit.OnCommandEnd += LogUnitCommandEnded;

            OnUnitSpawned?.Invoke(unit);
        }

        private void RemoveUnitFromList(IUnit unit)
        {
            _units.Remove(unit);
            _unitsExecutingCommand.Remove(unit);
            unit.OnDestroy -= RemoveUnitFromList;
            unit.OnCommandStart -= LogUnitCommandStarted;
            unit.OnCommandEnd -= LogUnitCommandEnded;

            OnUnitDespawned?.Invoke(unit);
        }

        private void LogUnitCommandStarted(IUnit unit, IUnitCommand command) =>
            _unitsExecutingCommand.Add(unit);

        private void LogUnitCommandEnded(IUnit unit, IUnitCommand command) =>
            _unitsExecutingCommand.Remove(unit);

        public IUnit Spawn(Territory territory, System.Type type)
        {
            IUnitOwner owner = (IUnitOwner)territory.Owner;
            if (owner == null)
                throw new UnityException("Territory owners must always be IUnitOwner and Player types.");
            UnitData data = GetUnitDataByUnit(owner, type);
            IUnit unit = UnitFactory.GenerateUnit(territory, data, type, UnitContainer, owner);
            InitUnit(unit);
            return unit;
        }

        public void Despawn(IUnit unit)
        {
            unit.DestroyUnit();
        }

        protected override void Enable() { }
        protected override void Disable() { }
    }
}