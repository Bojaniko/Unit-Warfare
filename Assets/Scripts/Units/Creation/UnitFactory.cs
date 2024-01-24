using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public static class UnitFactory
    {
        public static UnitIdentifier GenerateUnitIdentifier(GameObject prefab, TerritoryIdentifier starting_territory, UnitData data, Transform units_container)
        {
            GameObject instantiated = GameObject.Instantiate(prefab, starting_territory.transform.position, Quaternion.identity, units_container);
            UnitIdentifier identifier = instantiated.GetComponent<UnitIdentifier>();
            if (identifier == null)
            {
                Object.Destroy(instantiated);
                throw new UnityException("Error creating unit," +
                    "prefab must have a UnitIdentifier component attached!");
            }
            identifier.Data = data;
            identifier.SetStartingTerritory(starting_territory);
            return identifier;
        }

        public static IUnit GenerateUnit(Territory territory, UnitData data, Transform unit_container, IUnitTeamManager unit_manager)
        {
            _ = territory ?? throw new System.ArgumentException("Generated unit requires a Territory object.");
            _ = data ?? throw new System.ArgumentException("Generated unit requires a UnitData object.");
            _ = unit_container ?? throw new System.ArgumentException("Generated unit requires a global gameobject container for all units.");
            _ = unit_manager ?? throw new System.ArgumentException("Generated unit requires an IUnitTeamManager object.");

            GameObject go = GameObject.Instantiate(data.Prefab, territory.EMB.transform.position, Quaternion.identity, unit_container);
            foreach (System.Type t in _unitTypes)
            {
                System.Type[] generic_args = t.BaseType.GetGenericArguments();
                foreach (System.Type gt in generic_args)
                {
                    if (gt.Equals(data.GetType()))
                    {
                        object[] args = new object[] { territory, go, data, unit_manager };
                        IUnit unit = (IUnit)System.Activator.CreateInstance(t, args);
                        return unit;
                    }
                }
            }
            return null;
        }

        public static readonly System.Type[] _unitTypes = InitUnitTypes();

        private static System.Type[] InitUnitTypes()
        {
            List<System.Type> types = new();
            foreach (System.Type t in typeof(IUnit).Assembly.GetTypes())
            {
                bool isUnit = false;
                foreach (System.Type it in t.GetInterfaces())
                {
                    if (it.Equals(typeof(IUnit)))
                    {
                        isUnit = true;
                        break;
                    }
                }
                if (!isUnit)
                    continue;
                if (t.IsAbstract)
                    continue;
                if (t.IsInterface)
                    continue;
                types.Add(t);
            }
            return types.ToArray();
        }
    }
}