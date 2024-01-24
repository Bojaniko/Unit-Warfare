using UnitWarfare.Units;
using UnitWarfare.Core.Enums;

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

        public abstract Outcome[] GetOutcomes(IUnit unit, IUnitCommand[] commands);

        public record Outcome(IUnitCommand Command, AiBrainFeature Feature, Mode Mode);

        public record FeatureResponse(bool Valid, Mode Mode);

        protected class FeatureResponsePrototype<Unit, Command>
            where Unit : IUnit
            where Command : IUnitCommand
        {
            public FeatureResponsePrototype(System.Func<Unit, Command, FeatureResponse> validation, AiBrainFeature feature)
            {
                _feature = feature;
                _validation = validation;
            }

            private readonly AiBrainFeature _feature;
            public AiBrainFeature Feature => _feature;

            private readonly System.Func<Unit, Command, FeatureResponse> _validation;
            public FeatureResponse GetResponse(Unit unit, Command command) =>
                _validation.Invoke(unit, command);
        }
    }
}
