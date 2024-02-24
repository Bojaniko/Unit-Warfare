using System.Collections.Generic;

namespace UnitWarfare.Units
{
    public class UnitInitializerMediator
    {
        private readonly List<IUnitInitializer> _initializers;

        public UnitInitializerMediator(IUnitsHandler handler)
        {
            _initializers = new();

            _initializers.Add(new ActiveUnitInitializer(handler));
            _initializers.Add(new AntennaeInitializer(handler));
        }

        public void InitUnit(IUnit unit)
        {
            foreach (IUnitInitializer initializer in _initializers)
            {
                if (initializer.InitUnit(unit))
                    return;
            }
            throw new UnityEngine.UnityException($"There is no unit initializer for unit of type {unit.GetType()}.");
        }
    }
}