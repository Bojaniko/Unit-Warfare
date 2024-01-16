using UnityEngine;

namespace UnitWarfare.Territories
{
    [CreateAssetMenu(menuName = "Map/Map data")]
    public class MapData : ScriptableObject
    {
        [SerializeField] private Color _highlightedColor;
        public Color HighlightedColor => _highlightedColor;

        [SerializeField] private Color _activeColor;
        public Color ActiveColor => _activeColor;

        [SerializeField] private Color _playerOneColor;
        public Color PlayerOneColor => _playerOneColor;

        [SerializeField] private Color _playerTwoColor;
        public Color PlayerTwoColor => _playerTwoColor;

        [SerializeField] private Color _neutralPlayerColor;
        public Color NeutralPlayerColor => _neutralPlayerColor;

        [SerializeField, Range(1f, 3f)] private float _colorIntensity;
        public float ColorIntensity => _colorIntensity;

        public MapColorScheme GenerateColorScheme()
        {
            return new(_highlightedColor * _colorIntensity,
                _activeColor * _colorIntensity,
                _playerOneColor * _colorIntensity,
                _playerTwoColor * _colorIntensity,
                _neutralPlayerColor * _colorIntensity);
        }
    }
}