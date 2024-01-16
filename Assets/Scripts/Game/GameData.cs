using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Players;
using UnitWarfare.Cameras;
using UnitWarfare.Territories;

namespace UnitWarfare.Game
{
    [CreateAssetMenu(menuName = "Game Data")]
    class GameData : ScriptableObject
    {
        [SerializeField] UnitsData _unitsData;
        public UnitsData UnitsData => _unitsData;

        [SerializeField] CameraData _cameraData;
        public CameraData CameraData => _cameraData;

        [SerializeField] InputData _inputData;
        public InputData InputData => _inputData;

        [SerializeField] MatchData _matchData;
        public MatchData MatchData => _matchData;

        [SerializeField] MapData _mapData;
        public MapData MapData => _mapData;
    }
}