using System.Collections;

using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayersHandlerNetwork : PlayersHandler
    {
        public new record Config(PlayersHandler.Config Configuration, Photon.Realtime.Player NetworkPlayer);
        private readonly Config _config;

        private readonly PhotonView network_view;

        public PlayersHandlerNetwork(Config config, IGameStateHandler game_state_handler)
            : base(config.Configuration, game_state_handler)
        {
            _config = config;
            network_view = monoBehaviour.gameObject.AddComponent<PhotonView>();
            network_view.ViewID = GlobalValues.NETWORK_PLAYERS_HANDLER_VIEW_ID;
        }

        // TODO: Timer

        public override event System.Action<Player> OnActivePlayerChanged;

        protected override Player GeneratePlayerTwo()
        {
            return new PlayerNetwork(_config.Configuration.PlayerTwo, PlayerIdentification.OTHER_PLAYER, this);
        }

        protected override void OnPlayerExplicitMoveEnd(Player player)
        {
            if (player.Equals(LocalPlayer))
                network_view.RPC("SwitchToPlayer", RpcTarget.AllViaServer, _config.NetworkPlayer);
        }

        protected override void StartMatch()
        {
            if (PhotonNetwork.LocalPlayer.UserId.Equals(PhotonNetwork.CurrentRoom.MasterClientId))
                network_view.RPC("SwitchToPlayer", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer);
            else
                network_view.RPC("SwitchToPlayer", RpcTarget.AllViaServer, _config.NetworkPlayer);
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

            yield return new WaitForSeconds(_config.Configuration.Match.MaxTurnDuration);

            network_view.RPC("SwitchToPlayer", RpcTarget.AllViaServer, _config.NetworkPlayer);

            coroutine_playerOneRound = null;
        }

        public void SwitchToPlayer(Photon.Realtime.Player player)
        {
            if (player.Equals(PhotonNetwork.LocalPlayer))
            {
                if (LocalPlayer.IsActive)
                    return;
                StartLocalPlayerRound();
            }
            else if (player.Equals(_config.NetworkPlayer))
            {
                if (OtherPlayer.IsActive)
                    return;
                OnActivePlayerChanged?.Invoke(OtherPlayer);
            }
        }
    }
}