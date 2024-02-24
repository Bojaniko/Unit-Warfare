using System.Collections;

using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.AI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;

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

            m_timer = config.Configuration.Level.MaxRoundDuration;
            timerActive = false;
        }

        protected override Player GeneratePlayerOne()
        {
            return new PlayerLocal(_config.Configuration.PlayerLocal, this);
        }

        protected override Player GeneratePlayerTwo()
        {
            return new PlayerComputer(_config.Configuration.PlayerOther, this);
        }

        protected override void SubInitialize()
        {
            PlayerLocal.Config localConfig = new(gameStateHandler.GetHandler<InputHandler>().TapInput,
                gameStateHandler.GetHandler<CameraHandler>().MainCamera,
                gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>(),
                gameStateHandler.GetHandler<UIHandler>().GetUIHandler<UnitDisplay>(),
                gameStateHandler.GetHandler<UnitsHandler>());
            ((PlayerLocal)LocalPlayer).Configure(localConfig);

            PlayerComputer.Config aiConfig = new(_config.AiData, gameStateHandler.GetHandler<UnitsHandler>());
            ((PlayerComputer)OtherPlayer).Configure(aiConfig);
        }

        private bool timerActive = false;
        private float m_timer;
        public float Timer => m_timer;

        public override event System.Action<Player> OnActivePlayerChanged;

        protected override void StartMatch()
        {
            OnActivePlayerChanged?.Invoke(LocalPlayer);
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
            if (LocalPlayer.IsActive && !_unitsHandler.HasMovableUnits(LocalPlayer)
                || OtherPlayer.IsActive && !_unitsHandler.HasMovableUnits(OtherPlayer))
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
            if (LocalPlayer.IsActive)
            {
                player_active = OtherPlayer;
                OnActivePlayerChanged?.Invoke(OtherPlayer);
            }
            else
            {
                player_active = LocalPlayer;
                OnActivePlayerChanged?.Invoke(LocalPlayer);
            }
            timerActive = true;
            m_timer = _config.Configuration.Level.MaxRoundDuration;
            System.Func<float> timerDelegate = () => { return Timer; };
            ui_matchProgress.Show(new(player_active.Name, timerDelegate, true));
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