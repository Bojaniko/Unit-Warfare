

namespace UnitWarfare.Units
{
    public interface IUnitTeamManager
    {
        public delegate void UnitOwnerEventHandler();
        public event UnitOwnerEventHandler OnRoundStarted;
        public event UnitOwnerEventHandler OnRoundEnded;
    }
}