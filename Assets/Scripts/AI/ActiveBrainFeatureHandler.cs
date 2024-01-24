using System.Collections.Generic;

using UnitWarfare.Units;
using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    public class ActiveBrainFeatureHandler : BrainFeatureHandler
    {
        private List<FeatureResponsePrototype<IActiveUnit, UnitCommand<ActiveCommandOrder>>> _featureResponses;

        public ActiveBrainFeatureHandler()
        {
            GenerateFeatures();
        }

        private void GenerateFeatures()
        {
            _featureResponses = new();

            // ### AGRESSION ### \\
            System.Func<IActiveUnit, UnitCommand<ActiveCommandOrder>, FeatureResponse> agression = (unit, command) =>
                new(command.Order.Equals(ActiveCommandOrder.ATTACK), Mode.REDUCE);
            _featureResponses.Add(new(agression, AiBrainFeature.AGRESSIVE));

            // ### CONQUERING ### \\
            System.Func<IActiveUnit, UnitCommand<ActiveCommandOrder>, FeatureResponse> conquering = (unit, command) =>
            {
                if (command.Order.Equals(ActiveCommandOrder.ATTACK))
                    return new(true, Mode.REDUCE);
                if (command.Order.Equals(ActiveCommandOrder.MOVE))
                {
                    if (!command.Target.Territory.Owner.OwnerIdentification.Equals(unit.Owner) &&
                    !command.Target.Territory.Owner.OwnerIdentification.Equals(PlayerIdentification.NEUTRAL))
                        return new(true, Mode.INCREASE);
                }
                return new(false, Mode.UNALTER);
            };
            _featureResponses.Add(new(conquering, AiBrainFeature.CONQUERING));

            // ### TEAMPLAY ### \\
            System.Func<IActiveUnit, UnitCommand<ActiveCommandOrder>, FeatureResponse> teamplay = (unit, command) =>
            {
                if (command.Order.Equals(ActiveCommandOrder.JOIN))
                    return new(true, Mode.REDUCE);
                if (command.Order.Equals(ActiveCommandOrder.MOVE))
                {
                    if (command.Target.Territory.Owner.OwnerIdentification.Equals(unit.Owner))
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
            _featureResponses.Add(new(teamplay, AiBrainFeature.TEAMPLAY));

            // ### COWARDICE ### \\
            System.Func<IActiveUnit, UnitCommand<ActiveCommandOrder>, FeatureResponse> cowardice = (unit, command) =>
            {
                if (command.Target.Territory.Owner.OwnerIdentification.Equals(unit.Owner))
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
            _featureResponses.Add(new(cowardice, AiBrainFeature.COWARDICE));

            // ### EXPANDIONARY ### \\
            System.Func<IActiveUnit, UnitCommand<ActiveCommandOrder>, FeatureResponse> expandionary = (unit, command) =>
            {
                if (command.Order.Equals(ActiveCommandOrder.MOVE) && command.Target.Territory.Owner.OwnerIdentification.Equals(PlayerIdentification.NEUTRAL))
                    return new(true, Mode.REDUCE);
                return new(false, Mode.UNALTER);
            };
            _featureResponses.Add(new(expandionary, AiBrainFeature.EXPANDIONARY));
        }

        public override Outcome[] GetOutcomes(IUnit unit, IUnitCommand[] commands)
        {
            if (!(unit is IActiveUnit))
                return new Outcome[0];

            List<Outcome> outcomes = new();

            foreach (IUnitCommand command in commands)
            {
                UnitCommand<ActiveCommandOrder> order = command as UnitCommand<ActiveCommandOrder>;
                if (order == null)
                    continue;

                foreach (FeatureResponsePrototype<IActiveUnit, UnitCommand<ActiveCommandOrder>> responsePrototype in _featureResponses)
                {
                    FeatureResponse response = responsePrototype.GetResponse(unit as IActiveUnit, command as UnitCommand<ActiveCommandOrder>);
                    if (response.Valid)
                        outcomes.Add(new Outcome(command, responsePrototype.Feature, response.Mode));
                }
            }

            return outcomes.ToArray();
        }
    }
}