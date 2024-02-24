using System.Collections;

using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Core.Global;
using ExitGames.Client.Photon;

namespace UnitWarfare.Players
{
    public class PlayersHandlerNetwork : PlayersHandler, IOnEventCallback
    {
        public new record Config(PlayersHandler.Config Configuration, Photon.Realtime.Player NetworkPlayer);
        private readonly Config _config;

        public PlayersHandlerNetwork(Config config, IGameStateHandler game_state_handler)
            : base(config.Configuration, game_state_handler)
        {
            _config = config;
            PhotonNetwork.AddCallbackTarget(this);
        }

        // TODO: Timer

        public override event System.Action<Player> OnActivePlayerChanged;

        protected override Player GeneratePlayerOne()
        {
            return new PlayerLocalNetwork(_config.Configuration.PlayerLocal, this);
        }

        protected override Player GeneratePlayerTwo()
        {
            return new PlayerNetwork(_config.Configuration.PlayerOther, this);
        }

        protected override void SubInitialize()
        {
            PlayerLocal.Config localConfig = new(gameStateHandler.GetHandler<InputHandler>().TapInput,
                gameStateHandler.GetHandler<CameraHandler>().MainCamera,
                gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>(),
                gameStateHandler.GetHandler<UIHandler>().GetUIHandler<UnitDisplay>(),
                gameStateHandler.GetHandler<UnitsHandler>());
            ((PlayerLocal)LocalPlayer).Configure(localConfig);
        }

        protected override void OnPlayerExplicitMoveEnd(Player player)
        {
            if (player.Equals(LocalPlayer))
                SwitchActiveNetworkPlayer(_config.NetworkPlayer);
        }

        protected override void StartMatch()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(PhotonNetwork.CurrentRoom.MasterClientId))
                SwitchActiveNetworkPlayer(PhotonNetwork.LocalPlayer);
        }

        private Coroutine coroutine_playerOneRound;

        private void StartLocalPlayerRound()
        {
            if (coroutine_playerOneRound != null)
                monoBehaviour.StopCoroutine(coroutine_playerOneRound);
            coroutine_playerOneRound = monoBehaviour.StartCoroutine(LocalPlayerRoundRoutine());
        }

        private IEnumerator LocalPlayerRoundRoutine()
        {
            OnActivePlayerChanged?.Invoke(LocalPlayer);

            yield return new WaitForSeconds(_config.Configuration.Level.MaxRoundDuration);

            SwitchActiveNetworkPlayer(_config.NetworkPlayer);

            coroutine_playerOneRound = null;
        }

        private void SwitchActiveNetworkPlayer(Photon.Realtime.Player player)
        {
            RaiseEventOptions options = RaiseEventOptions.Default;
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_SWITCH_PLAYER_CODE, player, options, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (!photonEvent.Code.Equals(GlobalValues.NETWORK_SWITCH_PLAYER_CODE))
                return;
            Photon.Realtime.Player player = photonEvent.CustomData as Photon.Realtime.Player;
            if (player == null)
                return;
            if (player.ActorNumber.Equals(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                if (LocalPlayer.IsActive)
                    return;
                StartLocalPlayerRound();
            }
            else if (player.ActorNumber.Equals(_config.NetworkPlayer.ActorNumber))
            {
                if (OtherPlayer.IsActive)
                    return;
                OnActivePlayerChanged?.Invoke(OtherPlayer);
            }
        }
    }
}