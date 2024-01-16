namespace UnitWarfare.Core.Enums
{
    public enum PassiveCommandOrder
    {
        CALL,
        REPAIR,
        NONE
    }

    public enum ActiveCommandOrder
    {
        ATTACK,
        JOIN,
        MOVE,
        CANCEL
    }

    public enum GameType
    {
        TEST,
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
        PAUSED,
        PLAYING
    }

    public enum GameState
    {
        LOADING,
        PLAYING,
        TESTING
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
        CONQUERING
    }
}