using UnityEngine;

using UnitWarfare.Core.Global;

namespace UnitWarfare.Territories
{
    public class TerritoryIdentifier : MonoBehaviour
    {
        [SerializeField, HideInInspector] private PlayerIdentification _owner = PlayerIdentification.NEUTRAL;
        public PlayerIdentification Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        [SerializeField, HideInInspector] private TerritoryData _territoryData;
        public TerritoryData TerritoryData => _territoryData;
        public void SetTerritoryData(TerritoryData territory_data)
        {
            _territoryData = territory_data;
            foreach (Transform c in transform)
            {
                if (c.CompareTag("TileModel"))
                    c.GetComponent<MeshRenderer>().material = _territoryData.Material;
            }
        }

        [SerializeField, HideInInspector] private TileData _tileData;
        public TileData TileData
        {
            get
            {
                return _tileData;
            }
            internal set
            {
                _tileData = value;
            }
        }
    }
}