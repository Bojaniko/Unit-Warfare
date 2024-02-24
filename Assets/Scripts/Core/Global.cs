namespace UnitWarfare.Core.Global
{
    public static class GlobalValues
    {
        public const int NETWORK_MAX_RESPONSE_DELAY_SECONDS = 20;

        public const byte NETWORK_GAME_STARTED_CODE = 0;
        public const byte NETWORK_GAME_LOADED_CODE = 1;
        public const byte NETWORK_SWITCH_PLAYER_CODE = 2;
        public const byte NETWORK_UNIT_COMMAND_CODE = 3;
        public const byte NETWORK_SPAWN_UNIT_CODE = 4;
        public const byte NETWORK_SPAWN_UNIT_CONFIRMED_CODE = 5;
        public const byte NETWORK_DESPAWN_UNIT_CODE = 6;
        public const byte NETWORK_DESPAWN_UNIT_CONFIRMED_CODE = 7;

        public const string MAP_UNITS_CONTAINER = "UNITS";
    }

    public enum GameType
    {
        LOCAL,
        NETWORK
    }

    public enum ActiveCommandOrder
    {
        ATTACK,
        JOIN,
        MOVE,
        CANCEL
    }

    public enum AntennaeCommandOrder
    {
        GENERATE_UNIT,
        SKIP,
        CANCEL
    }

    public enum LoadingGameState
    { 
        PRE,
        POST,
        LOAD,
        FINAL
    }

    public enum PlayingGameState
    {
        LOADING,
        PLAYING,
        ENDED
    }

    public enum PlayerIdentifiers
    {
        PLAYER_ONE,
        PLAYER_TWO,
        NEUTRAL
    }

    public enum AiBrainFeature
    {
        AGRESSIVE,
        PASSIVE,
        TEAMPLAY,
        COWARDICE,
        CONQUERING,
        EXPANDIONARY,
        SCOUTING
    }
}