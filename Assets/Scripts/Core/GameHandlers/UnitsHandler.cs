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

        private readonly UnitSpawner m_spawner;
        public UnitSpawner Spawner => m_spawner;

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

        public UnitsHandler(UnitCombinations combinations, IGameStateHandler handler) : base(handler)
        {
            if (handler.GameType.Equals(GameType.NETWORK))
                m_spawner = new NetworkUnitSpawner(this);
            else
                m_spawner = new LocalUnitSpawner(this);

            _interactions = new(new(combinations.Combinations));

            m_unitsContainer = GameObject.Find(GlobalValues.MAP_UNITS_CONTAINER).transform;
            if (m_unitsContainer == null)
                throw new UnityException($"Game requires '{GlobalValues.MAP_UNITS_CONTAINER}' GameObject in the scene to store all Units.");
        }

        protected override void Initialize()
        {
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
                _units.Add(u);
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

        public bool UnitExecutingCommand => _unitsExecutingCommands.Count > 0;

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

        private List<IUnit> _unitsExecutingCommands;

        private void InitUnit(IUnit unit)
        {
            _units.Add(unit);

            _unitsExecutingCommands = new();

            unit.OnDestroy += HandleUnitDestroy;
            unit.OnCommandStart += HandleUnitCommandStart;
            unit.OnCommandEnd += HandleUnitCommandEnd;
        }

        private void HandleUnitDestroy(IUnit unit)
        {
            _unitsExecutingCommands.Remove(unit);

            unit.OnDestroy -= HandleUnitDestroy;
            unit.OnCommandStart -= HandleUnitCommandStart;
            unit.OnCommandEnd -= HandleUnitCommandEnd;

            unit.OccupiedTerritory.SetInteractable(true);
            unit.OccupiedTerritory.Deocuppy();

            _units.Remove(unit);
        }

        private void HandleUnitCommandStart(IUnit unit, IUnitCommand command)
        {
            _unitsExecutingCommands.Add(unit);
        }

        private void HandleUnitCommandEnd(IUnit unit, IUnitCommand command)
        {
            _unitsExecutingCommands.Remove(unit);
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