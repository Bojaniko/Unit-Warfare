using System.Collections.Generic;

using UnityEngine;

namespace UnitWarfare.Units
{
    [CreateAssetMenu(menuName = "Units/Data")]
    public class UnitsData : ScriptableObject
    {
        private List<UnitData> c_loadedData;
        public UnitData[] AllData => c_loadedData.ToArray();

        public UnitData GetDataByUnit(System.Type type)
        {
            System.Type data = type.GetProperty("Data").PropertyType;
            return GetData(data);
        }

        public UnitData GetDataByUnit<Unit>()
            where Unit : IUnit
        {
            System.Type data = typeof(Unit).GetProperty("Data").PropertyType;
            return GetData(data);
        }

        public UnitData GetData(System.Type type)
        {
            foreach (UnitData data in c_loadedData)
            {
                if (data.GetType().Equals(type))
                    return data;
            }
            return null;
        }

        public Data GetData<Data>()
            where Data : UnitData
        {
            foreach (UnitData data in c_loadedData)
            {
                if (data.GetType().Equals(typeof(Data)))
                    return data as Data;
            }
            return null;
        }


        private void OnValidate()
        {
            Awake();
        }

        private void Awake()
        {
            List<System.Type> unitTypes = new();
            foreach (System.Type type in typeof(IUnit).Assembly.GetTypes())
            {
                if (type.IsAbstract)
                    continue;
                if (type.IsInterface)
                    continue;
                if (type.GetInterface(typeof(IUnit).Name) != null)
                    unitTypes.Add(type);
            }

            if (_dataContainer == null)
                _dataContainer = new();
            foreach (System.Type t in unitTypes)
            {
                bool containsType = false;
                foreach (DataContainer dc in _dataContainer)
                {
                    if (dc.Unit.Contains(t.Name))
                    {
                        containsType = true;
                        break;
                    }
                }
                if (!containsType)
                    _dataContainer.Add(new(t.GetProperty("Data").PropertyType, t.Name));
            }

            c_loadedData = new();
            foreach (DataContainer dc in _dataContainer)
            {
                c_loadedData.Add(dc.Data);
            }
        }

        [SerializeField] private List<DataContainer> _dataContainer;

        [System.Serializable]
        public class DataContainer
        {
            [SerializeField] public string Unit;
            [SerializeField] public string Type;
            [SerializeField] public UnitData Data;

            public DataContainer(System.Type type, string unit)
            {
                Unit = unit;
                Type = $"{type.Namespace}.{type.Name}";
            }
        }
    }
}