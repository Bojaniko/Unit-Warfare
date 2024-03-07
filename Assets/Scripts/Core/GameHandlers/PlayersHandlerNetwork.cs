using System.Collections;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using NetworkPlayer = Photon.Realtime.Player;

using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Core;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayersHandlerNetwork : PlayersHandler, IOnEventCallback
    {
        private readonly PlayerLocal m_playerLocal;
        public override Player LocalPlayer => m_playerLocal;
        private readonly PlayerNetwork m_playerNetwork;
        public override Player OtherPlayer => m_playerNetwork;

        public new record Config(PlayersHandler.Config Configuration, NetworkPlayer NetworkPlayer);
        private readonly Config _config;

        public PlayersHandlerNetwork(Config config, IGameStateHandler game_state_handler)
            : base(config.Configuration, game_state_handler)
        {
            _config = config;
            m_playerLocal = new PlayerLocal(_config.Configuration.PlayerLocal, this);
            m_playerNetwork = new PlayerNetwork(_config.Configuration.PlayerOther, this);
            PhotonNetwork.AddCallbackTarget(this);
            PhotonNetwork.AddCallbackTarget(m_playerNetwork);
        }

        // TODO: Timer

        public override event System.Action<Player> OnActivePlayerChanged;

        protected override void SubInitialize()
        {
            PlayerLocal.Config localConfig = new(gameStateHandler.GetHandler<InputHandler>().TapInput,
                gameStateHandler.GetHandler<CameraHandler>().MainCamera,
                gameStateHandler.GetHandler<UIHandler>().GetComponent<MatchProgress>(),
                gameStateHandler.GetHandler<UIHandler>().GetUIHandler<UnitDisplay>(),
                gameStateHandler.GetHandler<UnitsHandler>(), true);
            ((PlayerLocal)LocalPlayer).Configure(localConfig);

            PlayerNetwork.Config networkConfig = new(gameStateHandler.GetHandler<UnitsHandler>(),
                gameStateHandler.GetHandler<TerritoryManager>(),
                _config.NetworkPlayer);
            ((PlayerNetwork)OtherPlayer).Configure(networkConfig);
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

        private void SwitchActiveNetworkPlayer(NetworkPlayer player)
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