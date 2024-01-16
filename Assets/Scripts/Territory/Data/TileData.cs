using UnityEngine;

namespace UnitWarfare.Territories
{
    [CreateAssetMenu(menuName = "Map/Tile Data")]
    public class TileData : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

        [SerializeField, Tooltip("The duration (s) it takes a tile to alternate between selection and idle.")] private float _selectionDuration = 1.5f;
        /// <summary>
        /// The duration (s) it takes a tile to alternate between selection and idle.
        /// </summary>
        public float SelectionDuration => _selectionDuration;

        [SerializeField, Tooltip("The farthest point describing the cirlce around the tile")] private float _tileRadius = 0.5f;
        /// <summary>
        /// The farthest point describing the cirlce around the tile.
        /// </summary>
        public float TileRadius => _tileRadius;

        [SerializeField, Tooltip("Positions of surrounding tiles")] private Vector2[] _tilePositions;
        /// <summary>
        /// Positions of surrounding tiles
        /// </summary>
        public Vector2[] TilePositions => _tilePositions;

        [SerializeField] private TerritoryData[] _territoryTypes;
        public TerritoryData[] TerritoryTypes => _territoryTypes;
    }
}