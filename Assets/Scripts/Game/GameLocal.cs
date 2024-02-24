using UnitWarfare.AI;
using UnitWarfare.Core.Global;
using UnitWarfare.Players;
using UnitWarfare.Units;

namespace UnitWarfare.Game
{
    public class GameLocal : GameBase
    {
        public new record Config(GameBase.Config Configuration, AiBrainData AiData);

        private readonly Config _config;
        private readonly AiBrainData _aiData;

        public override GameType GameType => GameType.LOCAL;

        public GameLocal(Config config)
            : base(config.Configuration)
        {
            _config = config;
            _aiData = _config.AiData;
        }

        protected override void OnLoadFinished()
        {
            stateMachine_play.SetState(PlayingGameState.PLAYING);
        }

        protected override PlayersHandler GeneratePlayersHandler()
        {
            PlayerData neutral = new("Neutral", _config.Configuration.Data.AllyNation);
            PlayerData playerOne = new("Player", _config.Configuration.Data.AllyNation);
            PlayerData playeTwo = new("Bot", _config.Configuration.Data.AxisNation);
            PlayersHandler.Config configuration = new(_config.Configuration.Level, playerOne, playeTwo, neutral);
            PlayersHandlerLocal.Config config = new(configuration, _aiData);
            return new PlayersHandlerLocal(config, this);
        }
    }
}