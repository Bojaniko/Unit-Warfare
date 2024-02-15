using System.Collections;

using UnityEngine;

using UnitWarfare.AI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayersHandlerLocal : PlayersHandler
    {
        public new record Config(PlayersHandler.Config Configuration, AiBrainData AiData);
        private readonly Config _config;

        public PlayersHandlerLocal(Config config, IGameStateHandler game_state_handler)
            : base(config.Configuration, game_state_handler)
        {
            _config = config;

            m_timer = config.Configuration.Match.MaxTurnDuration;
            timerActive = false;
        }

        protected override Player GeneratePlayerTwo()
        {
            return new PlayerComputer(gameStateHandler.GetHandler<UnitsHandler>(), _config.AiData, _config.Configuration.PlayerTwo, PlayerIdentification.OTHER_PLAYER, this);
        }

        private bool timerActive = false;
        private float m_timer;
        public float Timer => m_timer;

        public override event IPlayerHandler.PlayerEventHandler OnActivePlayerChanged;

        protected override void StartMatch()
        {
            OnActivePlayerChanged?.Invoke(PlayerOne);
            timerActive = true;
            StartPlayerTimer();
        }

        protected override void OnUpdate()
        {
            if (timerActive)
            {
                m_timer -= 1f * Time.deltaTime;
                if (m_timer <= 0)
                    timerActive = false;
            }

            if (_unitsHandler.UnitExecutingCommand)
                return;
            if (PlayerOne.IsActive && !_unitsHandler.HasMovableUnits(PlayerIdentification.PLAYER)
                || PlayerTwo.IsActive && !_unitsHandler.HasMovableUnits(PlayerIdentification.OTHER_PLAYER))
                SwitchActivePlayer();
        }

        private Coroutine coroutine_mainLoop;

        private IEnumerator MainPlayersLoop()
        {
            yield return new WaitUntil(() => !timerActive);

            ui_matchProgress.Hide();

            Debug.Log("Switching player");

            SwitchActivePlayer();
        }

        private void StartPlayerTimer()
        {
            if (coroutine_mainLoop != null)
                StopCoroutine(coroutine_mainLoop);
            coroutine_mainLoop = StartCoroutine(MainPlayersLoop());
        }

        private Player player_active;

        private void SwitchActivePlayer()
        {
            if (PlayerOne.IsActive)
            {
                player_active = PlayerTwo;
                OnActivePlayerChanged?.Invoke(PlayerTwo);
            }
            else
            {
                player_active = PlayerOne;
                OnActivePlayerChanged?.Invoke(PlayerOne);
            }
            timerActive = true;
            m_timer = _config.Configuration.Match.MaxTurnDuration;
            System.Func<float> timerDelegate = () => { return Timer; };
            ui_matchProgress.Show(new(player_active.Name, player_active.Identification, timerDelegate));
            StartPlayerTimer();
        }

        protected override void OnPlayerExplicitMoveEnd(Player player)
        {
            if (!player.IsActive)
                return;
            SwitchActivePlayer();
        }
    }
}