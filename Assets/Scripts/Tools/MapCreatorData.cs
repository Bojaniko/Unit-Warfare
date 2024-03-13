using UnityEngine;
using UnityEditor;

using UnitWarfare.Units;
using UnitWarfare.Territories;

namespace UnitWarfare.Tools
{
    [CreateAssetMenu(menuName = "Tools/Map Creator Data")]
    public class MapCreatorData : ScriptableObject
    {
        [SerializeField] private SceneAsset m_mapTemplate;
        public SceneAsset MapTemplate => m_mapTemplate;

        [Header("Units")]

        [SerializeField] private UnitsData _unitsData;
        public UnitsData UnitsData => _unitsData;

        [SerializeField] private GameObject _activeUnitPrefab;
        public GameObject ActiveUnitPrefab => _activeUnitPrefab;

        [SerializeField] private Material _neutralUnitMaterial;
        public Material NeutralUnitMaterial => _neutralUnitMaterial;

        [SerializeField] private Material _playerUnitMaterial;
        public Material PlayerUnitMaterial => _playerUnitMaterial;

        [SerializeField] private Material _otherPlayerUnitMaterial;
        public Material OtherPlayerUnitMaterial => _otherPlayerUnitMaterial;

        [Header("Tiles")]

        [SerializeField] private TileData _tileData;
        public TileData TileData => _tileData;

        public string TileTag => TilePrefab.tag;
        public GameObject TilePrefab => _tileData.Prefab;
        public Vector2[] TilePositions => _tileData.TilePositions;
    }
}