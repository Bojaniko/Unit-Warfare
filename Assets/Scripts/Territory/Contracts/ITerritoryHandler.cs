using Vector3 = UnityEngine.Vector3;

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

        public Territory[] Territories { get; }

        public Vector3 MapCenter { get; }
    }
}
