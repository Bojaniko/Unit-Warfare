using UnityEngine;

namespace UnitWarfare.Territories
{
    public static class TerritoryFactory
    {
        public static TerritoryIdentifier CreateTerritoryIdentifier(TileData tile_data, TerritoryData territory_data, Vector3 position, byte id)
        {
            Transform map = GameObject.Find("MAP").transform;
            if (map.Equals(null))
                map = new GameObject("MAP").transform;
            GameObject got = GameObject.Instantiate(tile_data.Prefab, position, Quaternion.identity, map);
            TerritoryIdentifier t = got.GetComponent<TerritoryIdentifier>();
            t.TileData = tile_data;
            t.SetID(id);
            t.SetTerritoryData(territory_data);
            return t;
        }

        public static Territory CreateTerritory(MapColorScheme color_scheme, TerritoryIdentifier identifier, ITerritoryHandler handler, ITerritoryOwner owner)
        {
            Territory t = new(color_scheme, identifier, handler, owner);
            return t;
        }
    }
}