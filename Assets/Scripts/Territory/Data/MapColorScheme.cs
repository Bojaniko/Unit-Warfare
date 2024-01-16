using UnityEngine;

namespace UnitWarfare.Territories
{
    public readonly struct MapColorScheme
    {
        private readonly Color _highlightedColor;
        public Color Highlighted => _highlightedColor;

        private readonly Color _activeColor;
        public Color Active => _activeColor;

        private readonly Color _playerOneColor;
        public Color PlayerOne => _playerOneColor;

        private readonly Color _playerTwoColor;
        public Color PlayerTwo => _playerTwoColor;

        private readonly Color _neutralPlayerColor;
        public Color NeutralPlayer => _neutralPlayerColor;

        public MapColorScheme(Color highlighted, Color active, Color player_one, Color player_two, Color neutral_player)
        {
            _highlightedColor = highlighted;
            _activeColor = active;
            _playerOneColor = player_one;
            _playerTwoColor = player_two;
            _neutralPlayerColor = neutral_player;
        }
    }
}