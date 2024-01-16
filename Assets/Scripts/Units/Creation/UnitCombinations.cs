using System.Collections.Generic;

using UnityEngine;

namespace UnitWarfare.Units
{
    [CreateAssetMenu(menuName = "Units/Combinations")]
    public class UnitCombinations : ScriptableObject
    {
        // ##### TYPES ##### \\

        public static System.Type[] UnitTypes => _unitTypes;
        private static System.Type[] _unitTypes = GetUnitTypes();
        private static System.Type[] GetUnitTypes()
        {
            List<System.Type> types = new();
            foreach (System.Type t in typeof(IUnit).Assembly.GetTypes())
            {
                if (t.GetInterface("IUnit") == null)
                    continue;
                if (t.IsAbstract)
                    continue;
                if (t.IsInterface)
                    continue;
                types.Add(t);
            }
            return types.ToArray();
        }

        private static System.Type GetUnitTypeFromName(string name)
        {
            foreach (System.Type t in _unitTypes)
            {
                if (t.Name.Equals(name))
                    return t;
            }
            return null;
        }

        // ##### COMBINATION ##### \\

        [System.Serializable]
        public class Combination
        {
            [SerializeField] public string UnitOne;
            [SerializeField] public string UnitTwo;
            [SerializeField] public string Result;
        }

        [SerializeField] private Combination[] _combinations;
        public Combination[] Combinations => _combinations;

        public class Manager
        {
            private readonly SyncedCombination[] _combinations;

            public Manager(Combination[] combinations)
            {
                _combinations = new SyncedCombination[combinations.Length];
                for (int i = 0; i < combinations.Length; i++)
                {
                    _combinations[i] = new(GetUnitTypeFromName(combinations[i].UnitOne),
                        GetUnitTypeFromName(combinations[i].UnitTwo),
                        GetUnitTypeFromName(combinations[i].Result));
                }
            }

            public System.Type GetResult(System.Type unit_one, System.Type unit_two)
            {
                foreach (SyncedCombination combination in _combinations)
                {
                    if (combination.UnitOne.Equals(unit_one) && combination.UnitTwo.Equals(unit_two) ||
                        combination.UnitOne.Equals(unit_two) && combination.UnitTwo.Equals(unit_one))
                        return combination.Result;
                }
                return null;
            }

            public bool IsCombinationValid(System.Type unit_one, System.Type unit_two)
            {
                foreach (SyncedCombination combination in _combinations)
                {
                    if (combination.UnitOne.Equals(unit_one) && combination.UnitTwo.Equals(unit_two) ||
                        combination.UnitOne.Equals(unit_two) && combination.UnitTwo.Equals(unit_one))
                        return true;
                }
                return false;
            }

            private record SyncedCombination(System.Type UnitOne, System.Type UnitTwo, System.Type Result);
        }
    }
}
