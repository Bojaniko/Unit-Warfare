namespace UnitWarfare.Territories
{
    public enum TerritoryHandlerState
    {
        PRE_LOADING,
        LOADING,
        READY
    }

    public interface ITerritoryHandler
    {
        public event System.Action<TerritoryHandlerState> OnStateChanged;

        public ITerritoryOwner Player { get; }
        public ITerritoryOwner OtherPlayer { get; }
        public ITerritoryOwner Neutral { get; }
    }
}
