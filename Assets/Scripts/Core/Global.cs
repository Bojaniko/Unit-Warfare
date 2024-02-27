namespace UnitWarfare.Core.Global
{
    public static class GlobalValues
    {
        public const byte NETWORK_REPEATED_EVENT_MAX_TRIES = 3;
        public const int NETWORK_REPEATED_EVENT_TRY_DELAY_MS = 500;

        public const byte NETWORK_GAME_STARTED_CODE = 0;
        public const byte NETWORK_GAME_LOADED_CODE = 1;
        public const byte NETWORK_SWITCH_PLAYER_CODE = 2;
        public const byte NETWORK_UNIT_COMMAND_CODE = 3;

        public const string MAP_UNITS_CONTAINER = "UNITS";
    }

    public enum GameType
    {
        LOCAL,
        NETWORK
    }

    public enum ActiveCommandOrder : byte
    {
        ATTACK,
        JOIN,
        MOVE,
        CANCEL
    }

    public enum AntennaeCommandOrder : byte
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