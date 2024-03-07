namespace UnitWarfare.Core
{
    public interface IGameStats
    {
        public int PlayerLocalManpower { get; }
        public int PlayerOtherManpower { get; }

        public int MaxManpower { get; }

        public event System.Action<int> OnPlayerLocalPointsChanged;
        public event System.Action<int> OnPlayerOtherPointsChanged;
    }
}