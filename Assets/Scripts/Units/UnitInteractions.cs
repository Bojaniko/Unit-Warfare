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
            List<IUnitCommand> commands = new();
            foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
            {
                UnitTarget target = new(t);
                IUnitCommand command = GenerateCommand(unit, target);
                if (command != null)
                    commands.Add(command);
            }
            return commands.ToArray();
        }

        public IUnitCommand GenerateCommand(IUnit unit, UnitTarget target)
        {
            if (unit.CurrentCommand != null)
                return null;
            if (target.Unit != null && target.Unit.CurrentCommand != null)
                return null;
            if (target.Territory == null)
                return null;
            if (!target.Territory.Interactable)
                return null;
            if (!unit.OccupiedTerritory.IsNeighbor(target.Territory))
                return null;


            if (unit is IActiveUnit)
            {
                UnitCommand<ActiveCommandOrder> command = GenerateActiveCommand(unit as IActiveUnit, target);
                UnityEngine.Debug.Log($"{unit} command is {command.ToString()}");
                return command;
            }

            return null;
        }

        private UnitCommand<ActiveCommandOrder> GenerateActiveCommand(IActiveUnit unit, UnitTarget target)
        {
            if (unit.OccupiedTerritory.Equals(target.Territory))
                UnityEngine.Debug.Log("Same territory");
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
