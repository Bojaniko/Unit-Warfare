namespace UnitWarfare.Core.Global
{
    public static class GlobalValues
    {
        public const string PLAYER_HOST_PROPERTY = "HostPlayer";
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

    public enum GameType
    {
        PLAYER_V_PLAYER,
        PLAYER_V_COMPUTER
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

    public enum PlayerIdentification
    {
        PLAYER,
        OTHER_PLAYER,
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