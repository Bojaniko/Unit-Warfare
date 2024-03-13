using UnityEngine;
using UnityEngine.SceneManagement;

using AudioManager = UnitWarfare.Audio.AudioManager;
using NetworkHandler = UnitWarfare.Network.NetworkHandler;

using UnitWarfare.Core;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Application
{
    public class ApplicationHandler : MonoBehaviour
    {
        private static ApplicationHandler s_instance;
        public static ApplicationHandler Instance => s_instance;

        private void Awake()
        {
            if (s_instance == null)
                throw new UnityException("Cannot have two instances of the ApplicationHandler class!");
            s_instance = this;
            DontDestroyOnLoad(this);

            NetworkHandler.CreateInstance();
            AudioManager.CreateInstance();

            SceneManager.sceneLoaded += HandleSceneLoad;
        }

        public void LoadMainMenu()
        {
            s_currentGame = null;
            // TODO: Load main menu
        }

        private static IGame s_currentGame;
        public static IGame CurrentGame => s_currentGame;

        public void LoadGameLevel(IGame game)
        {
            if (s_currentGame != null)
                throw new UnityException("Game already loaded.");
            if (game == null)
                throw new UnityException("Game cannot be null.");
            if (!game.LoadingState.Equals(LoadingGameState.PRE))
                throw new UnityException("Use LoadGameLevel to load games.");
            s_currentGame = game;
            DontDestroyOnLoad(game.EMB.gameObject);
            SceneManager.LoadScene(game.Level.SceneName);
        }

        private void HandleSceneLoad(Scene scene, LoadSceneMode mode)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                GameEncapsulatedMonoBehaviour gemb = go.GetComponent<GameEncapsulatedMonoBehaviour>();
                if (gemb != null)
                {
                    if (!gemb.Game.Equals(s_currentGame))
                        throw new UnityException("The game in the current scene does not match the game assigned in the ApplicationHandler.");
                    gemb.Game.Load();
                    break;
                }
            }
        }
    }
}
