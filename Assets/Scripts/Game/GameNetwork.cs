using UnitWarfare.Core.Global;
using UnitWarfare.Players;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace UnitWarfare.Game
{
    public class GameNetwork : GameBase, IOnEventCallback
    {
        public new record Config(Photon.Realtime.Player NetworkPlayer, GameBase.Config Configuration);
        private readonly Config _config;

        public override GameType GameType => GameType.NETWORK;

        public GameNetwork(Config config)
            : base(config.Configuration)
        {
            _config = config;
            network_playerLoaded = false;
            PhotonNetwork.AddCallbackTarget(this);
        }

        protected override PlayersHandler GeneratePlayersHandler()
        {
            // TODO: Player names;
            PlayerData neutral = new("Neutral", _config.Configuration.Data.AllyNation);
            PlayerData local = new("Player", _config.Configuration.Data.AllyNation);
            PlayerData network = new("Network", _config.Configuration.Data.AxisNation);
            PlayersHandler.Config configuration = new(_config.Configuration.Level, local, network, neutral);
            PlayersHandlerNetwork.Config config = new(configuration, _config.NetworkPlayer);
            PlayersHandlerNetwork handler = new(config, this);
            return handler;
        }

        private bool network_playerLoaded = false;

        protected override void OnLoadFinished()
        {
            if (network_playerLoaded)
                stateMachine_play.SetState(PlayingGameState.PLAYING);
            else
                PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_GAME_LOADED_CODE, PhotonNetwork.LocalPlayer, null, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (!photonEvent.Code.Equals(GlobalValues.NETWORK_GAME_LOADED_CODE))
                return;
            Photon.Realtime.Player player = photonEvent.CustomData as Photon.Realtime.Player;
            if (player == null)
                return;
            if (network_playerLoaded)
                return;
            if (player.ActorNumber.Equals(_config.NetworkPlayer.ActorNumber))
                network_playerLoaded = true;
            if (LoadingState.Equals(LoadingGameState.FINAL))
                stateMachine_play.SetState(PlayingGameState.PLAYING);
        }
    }
}