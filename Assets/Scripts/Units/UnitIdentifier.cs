using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class UnitIdentifier : MonoBehaviour
    {
        [SerializeField] private UnitData _data;
        public UnitData Data
        {
            get => _data;
            set
            {
                _data = value;
            }
        }

        [SerializeField, HideInInspector] private TerritoryIdentifier _startingTerritory;
        public TerritoryIdentifier StartingTerritory => _startingTerritory;

        public void SetStartingTerritory(TerritoryIdentifier territory)
        {
            _startingTerritory = territory;
            transform.position = _startingTerritory.transform.position;
        }
    }
}