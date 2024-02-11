using System.Collections.Generic;
using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.AI
{
    public class AntennaeFeatureHandler : BrainFeatureHandler
    {
        protected override FeatureResponsePrototype[] GenerateResponses()
        {
            List<FeatureResponsePrototype> responses = new();
            // ### AGRESSIVE ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> agression = (unit, command) =>
            {
                if (command.OrderRef.Equals(AntennaeCommandOrder.GENERATE_UNIT))
                {
                    foreach (Territory nt in command.Target.Territory.NeighborTerritories)
                    {
                        if (nt.Occupant != null && nt.Owner.Identification != unit.Owner)
                            return new(true, Mode.REDUCE);
                    }
                }
                return new(false, Mode.UNALTER);
            };
            responses.Add(new(agression, AiBrainFeature.AGRESSIVE));

            // ### PASSIVE ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> passive = (unit, command) =>
            {
                if (command.OrderRef.Equals(AntennaeCommandOrder.SKIP))
                {
                    bool hasEmptyTerritory = false;
                    foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
                    {
                        if (t.Occupant == null)
                            hasEmptyTerritory = true;
                    }
                    if (!hasEmptyTerritory)
                        return new(true, Mode.UNALTER);
                }
                return new(false, Mode.UNALTER);
            };
            responses.Add(new(passive, AiBrainFeature.PASSIVE));

            // ### TEAMPLAY ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> teamplay = (unit, command) =>
            {
                if (command.OrderRef.Equals(AntennaeCommandOrder.GENERATE_UNIT))
                {
                    foreach (Territory nt in command.Target.Territory.NeighborTerritories)
                    {
                        if (nt.Occupant != null && nt.Occupant.Owner.Equals(unit.Owner))
                            return new(true, Mode.REDUCE);
                    }
                }
                return new(false, Mode.UNALTER);
            };
            responses.Add(new(teamplay, AiBrainFeature.TEAMPLAY));

            System.Func<IUnit, IUnitCommand, FeatureResponse> expandionary = (unit, command) =>
            {
                if (command.OrderRef.Equals(AntennaeCommandOrder.GENERATE_UNIT))
                    return new(true, Mode.UNALTER);
                return new(false, Mode.UNALTER);
            };
            responses.Add(new(expandionary, AiBrainFeature.EXPANDIONARY));

            return responses.ToArray();
        }

        public override bool ValidateCommand(IUnitCommand command) =>
            command is UnitCommand<AntennaeCommandOrder>;

        public override bool ValidateUnit(IUnit unit) =>
            unit is Antennae;
    }
}