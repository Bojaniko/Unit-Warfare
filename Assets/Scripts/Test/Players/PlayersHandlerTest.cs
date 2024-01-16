using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

using UnitWarfare.UI;
using UnitWarfare.Input;
using UnitWarfare.Cameras;
using UnitWarfare.Players;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Test
{
    public class PlayersHandlerTest : MonoBehaviour
    {
        [SerializeField] private CameraData _cameraData;

        [SerializeField] private Camera _camera;

        [SerializeField] private InputData _inputData;

        [SerializeField] private MatchData _matchData;

        private PlayersHandler _handler;

        public void CreatePlayersHandler()
        {
            if (_matchData.Equals(null))
            {
                Debug.LogError("Match data not set!");
                return;
            }

            PlayersGameData data = new(_matchData, PlayerData.DefaultPlayer, PlayerData.DefaultEnemy);

            _handler = new PlayersHandler(data, TestGameStateHandler.CreateInstance());
            TestGameStateHandler.Instance.RegisterGameHandler(_handler);
            TestGameStateHandler.Instance.RegisterGameHandler(new UIHandler(TestGameStateHandler.Instance));
            TestGameStateHandler.Instance.RegisterGameHandler(new InputHandler(_inputData, TestGameStateHandler.Instance));
            TestGameStateHandler.Instance.RegisterGameHandler(new CameraHandler(_cameraData, TestGameStateHandler.Instance));

            TestGameStateHandler.Instance.SetGameState(GameState.LOADING);
            TestGameStateHandler.Instance.SetGameState(GameState.PLAYING);
        }
    }

    [CustomEditor(typeof(PlayersHandlerTest))]
    public class PlayersHandlerTestEditor : Editor
    {
        private PlayersHandlerTest tester;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Test!")) && tester == null)
            {
                tester = (PlayersHandlerTest)serializedObject.targetObject;
                tester.CreatePlayersHandler();
            }
        }
    }
}
