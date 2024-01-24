using UnitWarfare.Territories;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Units
{
    public delegate void UnitsEventHandler(IUnit unit);

    public interface IUnit : ITerritoryOccupant
    {
        /* TODO
         * public void Select();
         * public void Deselect();
         */

        public bool IsCommandActive { get; }
        public IUnitCommand CurrentCommand { get; }
        public void StartCommand(IUnitCommand command);

        public UnitData Data { get; }

        public bool MoveAvailable { get; }

        public void Damage(int amount);
        public int Health { get; }
        public int HealthPercentage { get; }
        public int Shield { get; }
        public int Attack { get; }

        public void DestroyUnit();
        public event UnitsEventHandler OnDestroy;

        public delegate void Command(IUnit unit, IUnitCommand command);
        public event Command OnCommandStart;
        public event Command OnCommandEnd;
    }
}