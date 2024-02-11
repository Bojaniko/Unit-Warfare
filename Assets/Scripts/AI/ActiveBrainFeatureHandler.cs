using System.Collections.Generic;

using UnitWarfare.Units;
using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    public class ActiveBrainFeatureHandler : BrainFeatureHandler
    {
        protected override FeatureResponsePrototype[] GenerateResponses()
        {
            List<FeatureResponsePrototype> featureResponses = new();

            // ### AGRESSION ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> agression = (unit, command) =>
                new(command.OrderRef.Equals(ActiveCommandOrder.ATTACK), Mode.REDUCE);
            featureResponses.Add(new(agression, AiBrainFeature.AGRESSIVE));

            // ### CONQUERING ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> conquering = (unit, command) =>
            {
                if (command.OrderRef.Equals(ActiveCommandOrder.ATTACK))
                    return new(true, Mode.REDUCE);
                if (command.OrderRef.Equals(ActiveCommandOrder.MOVE))
                {
                    if (!command.Target.Territory.Owner.Identification.Equals(unit.Owner) &&
                    !command.Target.Territory.Owner.Identification.Equals(PlayerIdentification.NEUTRAL))
                        return new(true, Mode.INCREASE);
                }
                return new(false, Mode.UNALTER);
            };
            featureResponses.Add(new(conquering, AiBrainFeature.CONQUERING));

            // ### TEAMPLAY ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> teamplay = (unit, command) =>
            {
                if (command.OrderRef.Equals(ActiveCommandOrder.JOIN))
                    return new(true, Mode.REDUCE);
                if (command.OrderRef.Equals(ActiveCommandOrder.MOVE))
                {
                    if (command.Target.Territory.Owner.Identification.Equals(unit.Owner))
                    {
                        foreach (Territory t in command.Target.Territory.NeighborTerritories)
                        {
                            if (t != unit.OccupiedTerritory && t.Occupant != null && t.Occupant.Owner.Equals(unit.Owner))
                                return new(true, Mode.MAXIMIZE);
                        }
                    }
                }
                return new(false, Mode.UNALTER);
            };
            featureResponses.Add(new(teamplay, AiBrainFeature.TEAMPLAY));

            // ### COWARDICE ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> cowardice = (unit, command) =>
            {
                if (command.Target.Territory.Owner.Identification.Equals(unit.Owner))
                {
                    bool canRetreat = false;
                    foreach (Territory t in command.Target.Territory.NeighborTerritories)
                    {
                        if (t.Occupant != null && !t.Occupant.Owner.Equals(unit.Owner) && !t.Occupant.Owner.Equals(PlayerIdentification.NEUTRAL))
                        {
                            canRetreat = true;
                            break;
                        }
                    }
                    if (canRetreat)
                        return new(true, Mode.UNALTER);
                }
                return new(false, Mode.UNALTER);
            };
            featureResponses.Add(new(cowardice, AiBrainFeature.COWARDICE));

            // ### EXPANDIONARY ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> expandionary = (unit, command) =>
            {
                if (command.OrderRef.Equals(ActiveCommandOrder.MOVE) && command.Target.Territory.Owner.Identification.Equals(PlayerIdentification.NEUTRAL))
                    return new(true, Mode.REDUCE);
                return new(false, Mode.UNALTER);
            };
            featureResponses.Add(new(expandionary, AiBrainFeature.EXPANDIONARY));

            // ### SCOUTING ### \\
            System.Func<IUnit, IUnitCommand, FeatureResponse> scouting = (unit, command) =>
            {
                if (command.OrderRef.Equals(ActiveCommandOrder.MOVE) && command.Target.Territory.Owner.Identification.Equals(unit.Owner))
                {
                    bool canScout = true;
                    foreach (Territory t in unit.OccupiedTerritory.NeighborTerritories)
                    {
                        if (t.Owner.Identification != unit.Owner)
                        {
                            canScout = false;
                            break;
                        }
                    }
                    if (canScout)
                        return new(true, Mode.REDUCE);
                }
                return new(false, Mode.UNALTER);
            };
            featureResponses.Add(new(scouting, AiBrainFeature.SCOUTING));

            return featureResponses.ToArray();
        }

        public override bool ValidateCommand(IUnitCommand command) =>
            command is UnitCommand<ActiveCommandOrder>;

        public override bool ValidateUnit(IUnit unit) =>
            unit is IActiveUnit;
    }
}