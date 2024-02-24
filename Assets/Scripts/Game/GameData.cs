using UnityEngine;

using UnitWarfare.UI;
using UnitWarfare.Units;
using UnitWarfare.Input;
using UnitWarfare.Players;
using UnitWarfare.Cameras;
using UnitWarfare.Territories;

namespace UnitWarfare.Game
{
    [CreateAssetMenu(menuName = "Game/Data")]
    public class GameData : ScriptableObject
    {
        [SerializeField] private UnitCombinations _combinations;
        public UnitCombinations Combinations => _combinations;

        [SerializeField] private Nation m_allyNation;
        public Nation AllyNation => m_allyNation;

        [SerializeField] private Nation m_axisNation;
        public Nation AxisNation => m_axisNation;

        [SerializeField] CameraData m_cameraData;
        public CameraData CameraData => m_cameraData;

        [SerializeField] InputData m_inputData;
        public InputData InputData => m_inputData;

        [SerializeField] MapColorSchemeData m_mapData;
        public MapColorSchemeData MapData => m_mapData;

        [SerializeField] private UIData m_uiData;
        public UIData UIData => m_uiData;
    }
}