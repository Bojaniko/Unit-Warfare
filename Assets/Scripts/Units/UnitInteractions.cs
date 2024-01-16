using System.Collections.Generic;

using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Units
{
    public class UnitInteractions
    {
        private readonly UnitCombinations.Manager _combinations;
        public UnitCombinations.Manager CombinationsManager => _combinations;

        public UnitInteractions(UnitCombinations.Manager combinations)
        {
            _combinations = combinations;
        }

        public IUnitCommand[] GenerateCommands(IUnit unit)
        {
            List<IUnitCommand> _commands = new();
            foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
            {
                UnitTarget target = new(t);
                IUnitCommand command = GenerateCommand(unit, target);
                if (command != null)
                    _commands.Add(command);
            }
            return _commands.ToArray();
        }

        public IUnitCommand GenerateCommand(IUnit unit, UnitTarget target)
        {
            if (target.Territory == null)
                return null;
            if (!unit.OccupiedTerritory.IsNeighbor(target.Territory))
                return null;
            if (unit is IActiveUnit)
                return GenerateActiveCommand(unit as IActiveUnit, target);
            return null;
        }

        private UnitCommand<ActiveCommandOrder> GenerateActiveCommand(IActiveUnit unit, UnitTarget target)
        {
            if (target.Unit == null)
                return new UnitCommand<ActiveCommandOrder>(ActiveCommandOrder.MOVE, target);
            else
            {
                if (unit.Owner.Equals(target.Unit.Owner) && _combinations.IsCombinationValid(unit.GetType(), target.Unit.GetType()))
                    return new UnitCommand<ActiveCommandOrder>(ActiveCommandOrder.JOIN, target);
                if (!unit.Owner.Equals(target.Unit.Owner))
                    return new UnitCommand<ActiveCommandOrder>(ActiveCommandOrder.ATTACK, target);
            }
            return new UnitCommand<ActiveCommandOrder>(ActiveCommandOrder.CANCEL, target);
        }
    }
}
