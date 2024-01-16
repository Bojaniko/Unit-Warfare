using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace UnitWarfare.Units
{
    [CreateAssetMenu(menuName = "Units/Data")]
    public class UnitsData : ScriptableObject
    {
        [SerializeField] private UnitCombinations _combinations;
        public UnitCombinations Combinations => _combinations;

        [Header("Active Units")]

        [SerializeField] private SoldierData _soldier;
        public SoldierData Soldier => _soldier;

        [SerializeField] private ArmoredUnitData _armored;
        public ArmoredUnitData ArmoredUnit => _armored;

        public UnitData[] GetAllData()
        {
            List<UnitData> datas = new();
            PropertyInfo[] pis = typeof(UnitsData).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (!pi.PropertyType.IsSubclassOf(typeof(UnitData)))
                    continue;
                datas.Add((UnitData)pi.GetValue(this));
            }
            return datas.ToArray();
        }

        public T GetData<T>(UnitsData data)
            where T : UnitData
        {
            PropertyInfo[] pis = typeof(UnitsData).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (!pi.PropertyType.Equals(typeof(T)))
                    continue;
                return (T)pi.GetValue(this);
            }
            return null;
        }
    }
}