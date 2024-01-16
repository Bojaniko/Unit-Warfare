using UnityEngine;

namespace UnitWarfare.Territories
{
    public static class TerritoryFactory
    {
        public static TerritoryIdentifier CreateTerritory(TileData tile_data, TerritoryData territory_data, Vector3 position)
        {
            Transform map = GameObject.Find("MAP").transform;
            if (map.Equals(null))
                map = new GameObject("MAP").transform;
            GameObject got = GameObject.Instantiate(tile_data.Prefab, position, Quaternion.identity, map);
            TerritoryIdentifier t = got.GetComponent<TerritoryIdentifier>();
            t.TileData = tile_data;
            t.SetTerritoryData(territory_data);
            return t;
        }
    }
}