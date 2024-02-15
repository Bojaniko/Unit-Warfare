using System.Collections.Generic;

using UnitWarfare.Units;
using UnitWarfare.Core.Global;

using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.AI
{
    public abstract class BrainFeatureHandler
    {
        public enum Mode
        {
            REDUCE,
            INCREASE,
            UNALTER,
            MAXIMIZE,
            MINIMIZE
        }

        protected BrainFeatureHandler()
        {
            featureResponses = GenerateResponses();
        }

        public abstract bool ValidateUnit(IUnit unit);
        public abstract bool ValidateCommand(IUnitCommand command);

        protected abstract FeatureResponsePrototype[] GenerateResponses();
        private readonly FeatureResponsePrototype[] featureResponses;

        public Outcome[] GetOutcomes(IUnit unit, IUnitCommand[] commands)
        {
            if (!ValidateUnit(unit))
                return new Outcome[0];

            List<Outcome> outcomes = new();

            foreach (IUnitCommand command in commands)
            {
                if (!ValidateCommand(command))
                    continue;

                foreach (FeatureResponsePrototype responsePrototype in featureResponses)
                {
                    FeatureResponse response = responsePrototype.GetResponse(unit, command);
                    if (response.Valid)
                        outcomes.Add(new Outcome(command, responsePrototype.Feature, response.Mode));
                }
            }

            return outcomes.ToArray();
        }

        public record Outcome(IUnitCommand Command, AiBrainFeature Feature, Mode Mode);

        public record FeatureResponse(bool Valid, Mode Mode);

        protected class FeatureResponsePrototype
        {
            public FeatureResponsePrototype(System.Func<IUnit, IUnitCommand, FeatureResponse> validation, AiBrainFeature feature)
            {
                m_feature = feature;
                m_validation = validation;
            }

            private readonly AiBrainFeature m_feature;
            public AiBrainFeature Feature => m_feature;

            private readonly System.Func<IUnit, IUnitCommand, FeatureResponse> m_validation;
            public FeatureResponse GetResponse(IUnit unit, IUnitCommand command) =>
                m_validation.Invoke(unit, command);
        }
    }
}
