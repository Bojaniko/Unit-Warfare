using System.Collections.Generic;

using Studio28.Utility;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Players;
using UnitWarfare.Network;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Territories
{
    public class TerritoryManager : GameHandler, ITerritoryHandler
    {
        private ITerritoryOwner m_player;
        public ITerritoryOwner Player => m_player;

        private ITerritoryOwner m_otherPlayer;
        public ITerritoryOwner OtherPlayer => m_otherPlayer;

        private ITerritoryOwner m_neutral;
        public ITerritoryOwner Neutral => m_neutral;

        private readonly MapColorSchemeData m_mapData;

        private readonly StateMachine<TerritoryHandlerState> stateController;
        public event System.Action<TerritoryHandlerState> OnStateChanged;

        public TerritoryManager(MapColorSchemeData map_data, IGameStateHandler game_state_handler)
            : base(game_state_handler, GameObject.Find(GlobalValues.MAP_TERRITORIES_CONTAINER))
        {
            if (gameObject == null)
                throw new UnityException($"GameObject container for territories with name {GlobalValues.MAP_TERRITORIES_CONTAINER} not found.\nAll territories must be parented to this GameObject.");
            m_mapData = map_data;
            stateController = new("Territory Manager State Controller", TerritoryHandlerState.PRE_LOADING);
            stateController.OnStateChanged += (state) => { OnStateChanged?.Invoke(state); };
        }

        protected override void OnFinalLoad()
        {
            RemoveIdentifiers();

            stateController.SetState(TerritoryHandlerState.READY);
        }

        protected override void Initialize()
        {
            RegisterOwners();

            CreateTerritories();

            stateController.SetState(TerritoryHandlerState.LOADING);
        }

        private void RegisterOwners()
        {
            PlayersHandler playersHandler = gameStateHandler.GetHandler<PlayersHandler>();

            m_player = playersHandler.LocalPlayer;
            m_otherPlayer = playersHandler.OtherPlayer;
            m_neutral = playersHandler.NeutralPlayer;
        }

        private List<Territory> _territories;
        public Territory[] Territories => _territories.ToArray();

        private void CreateTerritories()
        {
            _territories = new();
            _identifiers = new();

            MapColorScheme colorScheme;
            if (gameStateHandler.GameType.Equals(GameType.NETWORK))
                colorScheme = m_mapData.GenerateColorScheme(true);
            else
                colorScheme = m_mapData.GenerateColorScheme(false);

            for (int i = 0; i < transform.childCount; i++)
            {
                TerritoryIdentifier ti = transform.GetChild(i).GetComponent<TerritoryIdentifier>();
                if (ti == null)
                    continue;

                ITerritoryOwner owner = GetOwner(ti.Owner);

                Territory t = TerritoryFactory.CreateTerritory(colorScheme, ti, this, owner);
                foreach (Territory tt in _territories.ToArray())
                {
                    if (t.ID.Equals(tt.ID))
                        throw new UnityException("Map is not properly formated. Please use 'Unit Warfare/Reload Tile IDs'.");
                }
                _identifiers.Add(ti, t);
                _territories.Add(t);
            }
            if (_territories.Count == 0)
                throw new UnityException("Map is empty or data is invalid.");
            GetMapCenter();
        }

        private ITerritoryOwner GetOwner(PlayerIdentifiers identifier)
        {
            if (gameStateHandler.GameType.Equals(GameType.NETWORK))
            {
                if (NetworkHandler.Instance.IsHost)
                {
                    if (identifier.Equals(PlayerIdentifiers.PLAYER_ONE))
                        return Player;
                    else if (identifier.Equals(PlayerIdentifiers.PLAYER_TWO))
                        return OtherPlayer;
                    else
                        return Neutral;
                }
                else
                {
                    if (identifier.Equals(PlayerIdentifiers.PLAYER_ONE))
                        return OtherPlayer;
                    else if (identifier.Equals(PlayerIdentifiers.PLAYER_TWO))
                        return Player;
                    else
                        return Neutral;
                }
            }
            else
            {
                if (identifier.Equals(PlayerIdentifiers.PLAYER_ONE))
                    return Player;
                else if (identifier.Equals(PlayerIdentifiers.PLAYER_TWO))
                    return OtherPlayer;
                else
                    return Neutral;
            }
        }

        private void GetMapCenter()
        {
            float xSum = 0f;
            float ySum = 0f;
            float zSum = 0f;
            foreach(Territory t in _territories)
            {
                xSum += t.EMB.transform.position.x;
                ySum += t.EMB.transform.position.y;
                zSum += t.EMB.transform.position.z;
            }
            m_mapCenter = new Vector3(xSum / _territories.Count, ySum / _territories.Count, zSum / _territories.Count);
        }

        private Vector3 m_mapCenter;
        public Vector3 MapCenter => m_mapCenter;

        // ##### IDENTIFIERS ##### \\
        private Dictionary<TerritoryIdentifier, Territory> _identifiers;

        private void RemoveIdentifiers()
        {
            foreach (TerritoryIdentifier i in _identifiers.Keys)
                Object.Destroy(i);
            _identifiers = null;
        }

        // Identifiers are removed in the Final load stage
        public Territory GetByIdentifier(TerritoryIdentifier identifier)
        {
            if (_identifiers == null)
                return null;
            return _identifiers[identifier];
        }

        protected override void Enable() { }
        protected override void Disable() { }
    }
}