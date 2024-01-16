namespace UnitWarfare.Units
{
    public interface IUnitTeamManager
    {
        public delegate void UnitManagerEventHandler();
        public event UnitManagerEventHandler OnRoundStarted;
        public event UnitManagerEventHandler OnRoundEnded;
    }
}