using System.Collections.Generic;

using Studio28.Utility;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Players;

namespace UnitWarfare.Territories
{
    public class TerritoryManager : GameHandler, ITerritoryHandler
    {
        private ITerritoryOwner _player;
        public ITerritoryOwner Player => _player;

        private ITerritoryOwner _otherPlayer;
        public ITerritoryOwner OtherPlayer => _otherPlayer;

        private ITerritoryOwner _neutral;
        public ITerritoryOwner Neutral => _neutral;

        private readonly MapData _mapData;

        private StateMachine<TerritoryHandlerState> _stateController;
        public event TerritoryHandlerStateEventHandler OnStateChanged;

        public TerritoryManager(MapData map_data, IGameStateHandler game_state_handler)
            : base(game_state_handler, GameObject.Find("MAP"))
        {
            _mapData = map_data;
            _stateController = new("Territory Manager State Controller", TerritoryHandlerState.PRE_LOADING);
            _stateController.OnStateChanged += (state) => { OnStateChanged?.Invoke(state); };

            GetMapCenter();
        }

        protected override void OnFinalLoad()
        {
            RemoveIdentifiers();

            _stateController.SetState(TerritoryHandlerState.READY);
        }

        protected override void Initialize()
        {
            RegisterOwners();

            CreateTerritories();

            _stateController.SetState(TerritoryHandlerState.LOADING);
        }

        private void RegisterOwners()
        {
            PlayersHandler playersHandler = gameStateHandler.GetHandler<PlayersHandler>();

            _player = playersHandler.PlayerOne;
            _otherPlayer = playersHandler.PlayerTwo;
            _neutral = playersHandler.NeutralPlayer;
        }

        private List<Territory> _territories;
        private void CreateTerritories()
        {
            _territories = new();
            _identifiers = new();

            MapColorScheme colorScheme = _mapData.GenerateColorScheme();

            for (int i = 0; i < transform.childCount; i++)
            {
                TerritoryIdentifier ti = transform.GetChild(i).GetComponent<TerritoryIdentifier>();
                if (ti == null)
                    continue;
                Territory t = new(colorScheme, ti, this);
                _identifiers.Add(ti, t);
                _territories.Add(t);
            }

            if (_territories.Count == 0)
                throw new UnityException("Map is empty or data is invalid.");
        }

        private void GetMapCenter()
        {
            // TODO: Calculate map center
            _mapCenter = Vector3.zero;
        }

        private Vector3 _mapCenter;
        public Vector3 MapCenter => _mapCenter;

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
    }
}