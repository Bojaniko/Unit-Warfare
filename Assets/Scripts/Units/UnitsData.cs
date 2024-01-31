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

        private List<UnitData> c_loadedData;
        public UnitData[] AllData => c_loadedData.ToArray();

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
            List<System.Type> dataTypes = new();
            foreach (System.Type type in typeof(UnitData).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(UnitData)) && !type.IsAbstract)
                    dataTypes.Add(type);
            }
            if (_dataContainer == null)
                _dataContainer = new();
            foreach (System.Type t in dataTypes)
            {
                bool containsType = false;
                foreach (DataContainer dc in _dataContainer)
                {
                    if (dc.Type.Contains(t.Name))
                    {
                        containsType = true;
                        break;
                    }
                }
                if (!containsType)
                    _dataContainer.Add(new(t));
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
            [SerializeField] public string Type;
            [SerializeField] public UnitData Data;

            public DataContainer(System.Type type)
            {
                Type = $"{type.Namespace}.{type.Name}";
            }
        }
    }
}