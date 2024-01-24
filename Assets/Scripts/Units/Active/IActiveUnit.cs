using UnitWarfare.Core.Enums;

namespace UnitWarfare.Units
{
    public interface IActiveUnit : IUnit
    {
        public new delegate void Command(IActiveUnit unit, UnitTarget target);

        public event Command OnAttack;

        public event Command OnMove;

        public event Command OnJoin;
    }
}