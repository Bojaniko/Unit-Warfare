namespace UnitWarfare.Territories
{
    public enum TerritoryHandlerState
    {
        PRE_LOADING,
        LOADING,
        READY
    }

    public delegate void TerritoryHandlerStateEventHandler(TerritoryHandlerState state);

    public interface ITerritoryHandler
    {
        public event TerritoryHandlerStateEventHandler OnStateChanged;

        public ITerritoryOwner Player { get; }
        public ITerritoryOwner OtherPlayer { get; }
        public ITerritoryOwner Neutral { get; }
    }
}
