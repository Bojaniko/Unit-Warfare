using System.Collections;
using System.Collections.Generic;

using Photon.Realtime;
using Photon.Pun;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using UnitWarfare.AI;
using UnitWarfare.Units;
using UnitWarfare.Core;
using UnitWarfare.Players;
using UnitWarfare.Network;
using UnitWarfare.Game.UI;

namespace UnitWarfare.Game
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private GameStarterData m_data;

        private Canvas c_canvas;

        private Menu menu_active;

        private Menu menu_main;
        private Menu menu_ai;
        private Menu menu_settings;

        private Menu menu_lobby;
        private Menu menu_connecting;
        private Menu menu_connectionFailed;

        private LevelSelector level_selected;
        private string difficulty_selected;
        private string nation_selected;

        private const float TRANSITION_BETWEEN_MENUS = 0.4f;

        private const string CANVAS_GAMEOBJECT_NAME = "Canvas";
        private const string MAIN_MENU_UI_PATH = "UI/Main/main_menu";
        private const string SETTINGS_MENU_UI_PATH = "UI/Main/settings_menu";

        private const string LOBBY_MENU_UI_PATH = "UI/Main/lobby";
        private const string CONNECTING_MENU_UI_PATH = "UI/Main/connecting";
        private const string FAILED_CONNECTION_MENU_UI_PATH = "UI/Main/connection_failed";

        private const string LOBBY_CANCEL_BUTTON = "back";
        private const string LOBBY_SEARCHING_MENU = "searching";
        private const string LOBBY_STARTING_GAME_MENU = "found";
        private const string FAILED_CONNECTION_BACK_BUTTON = "back";

        private const string AI_MENU_UI_PATH = "UI/Main/ai_menu";
        private const string PLAY_WITH_AI_BUTTON = "play_ai";
        private const string PLAY_WITH_PLAYER_BUTTON = "play_player";
        private const string SETTINGS_BUTTON = "settings";

        private const string AI_LEVELS = "level_selector";
        private const string AI_SELECTOR = "ai_selector";
        private const string AI_NATION = "nations";
        private const string AI_PLAY_BUTTON = "play";
        private const string AI_RETURN_BUTTON = "back";

        private void Awake()
        {
            NetworkHandler.CreateInstance();

            InitMenus();

            menu_main.SetActive(true);
            menu_main.SetVisible(true);
            menu_active = menu_main;

            ExitGames.Client.Photon.PhotonPeer.RegisterType(typeof(NetworkUnitCommand), 0, NetworkUnitCommand.Serialize, NetworkUnitCommand.Deserialize);

            // TODO: Remove this
            PhotonNetwork.KeepAliveInBackground = 120;
        }

        private void InitMenus()
        {
            if (m_data == null)
                throw new UnityException("The game starter needs a data object assigned!");
            if (m_data.GameData == null)
                throw new UnityException("The game starter data needs a game data object assigned!");
            if (m_data.AiBrains.Length == 0)
                throw new UnityException("There is no AI brains data in the game started data!");
            if (m_data.Levels.Length == 0)
                throw new UnityException("There are no levels in the game starter data!");

            c_canvas = GameObject.Find(CANVAS_GAMEOBJECT_NAME).GetComponent<Canvas>();
            if (c_canvas == null)
                throw new UnityException($"There is no canvas in the scene with the name {CANVAS_GAMEOBJECT_NAME}.");

            VisualTreeAsset mainMenuVisuals = (VisualTreeAsset)Resources.Load(MAIN_MENU_UI_PATH);
            if (mainMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the main menu in the Resources folder with the path {MAIN_MENU_UI_PATH}.");
            GameObject mainMenuGO = new GameObject("MAIN_MENU");
            mainMenuGO.transform.parent = c_canvas.transform;
            UIDocument mainMenu = mainMenuGO.AddComponent<UIDocument>();
            mainMenu.visualTreeAsset = mainMenuVisuals;
            mainMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            menu_main = new(mainMenu);
            InitMainMenu();

            VisualTreeAsset aiMenuVisuals = (VisualTreeAsset)Resources.Load(AI_MENU_UI_PATH);
            if (aiMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the ai menu in the Resources folder with the path {MAIN_MENU_UI_PATH}.");
            GameObject aiMenuGO = new GameObject("AI_MENU");
            aiMenuGO.transform.parent = c_canvas.transform;
            UIDocument aiMenu = aiMenuGO.AddComponent<UIDocument>();
            aiMenu.visualTreeAsset = aiMenuVisuals;
            aiMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            menu_ai = new(aiMenu);
            InitAiMenu();

            VisualTreeAsset lobbyMenuVisuals = (VisualTreeAsset)Resources.Load(LOBBY_MENU_UI_PATH);
            if (lobbyMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the lobby menu in the Resources folder with the path {LOBBY_MENU_UI_PATH}.");
            GameObject lobbyMenuGO = new GameObject("LOBBY_MENU");
            lobbyMenuGO.transform.parent = c_canvas.transform;
            UIDocument lobbyMenu = lobbyMenuGO.AddComponent<UIDocument>();
            lobbyMenu.visualTreeAsset = lobbyMenuVisuals;
            lobbyMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            menu_lobby = new(lobbyMenu);
            InitLobbyMenu();

            VisualTreeAsset connectingMenuVisuals = (VisualTreeAsset)Resources.Load(CONNECTING_MENU_UI_PATH);
            if (connectingMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the connecting menu in the Resources folder with the path {CONNECTING_MENU_UI_PATH}.");
            GameObject connectingMenuGO = new GameObject("CONNECTING_MENU");
            connectingMenuGO.transform.parent = c_canvas.transform;
            UIDocument connectingMenu = connectingMenuGO.AddComponent<UIDocument>();
            connectingMenu.visualTreeAsset = connectingMenuVisuals;
            connectingMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            menu_connecting = new(connectingMenu);
            InitConnectingMenu();

            VisualTreeAsset connectionFailedMenuVisuals = (VisualTreeAsset)Resources.Load(FAILED_CONNECTION_MENU_UI_PATH);
            if (connectionFailedMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the connection failed menu in the Resources folder with the path {FAILED_CONNECTION_MENU_UI_PATH}.");
            GameObject failedConnectionMenuGO = new GameObject("FAILED_CONNECTION_MENU");
            failedConnectionMenuGO.transform.parent = c_canvas.transform;
            UIDocument failedConnectionMenu = failedConnectionMenuGO.AddComponent<UIDocument>();
            failedConnectionMenu.visualTreeAsset = connectionFailedMenuVisuals;
            failedConnectionMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            menu_connectionFailed = new(failedConnectionMenu);
            InitFailedConnectionMenu();

            // TODO: Menus
            /*
            VisualTreeAsset lobbyMenuVisuals = (VisualTreeAsset)Resources.Load(LOBBY_MENU_UI_PATH);
            if (lobbyMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the lobby menu in the Resources folder with the path {LOBBY_MENU_UI_PATH}.");
            GameObject lobbyMenuGO = new GameObject("LOBBY_MENU");
            lobbyMenuGO.transform.parent = c_canvas.transform;
            c_lobbyMenu = lobbyMenuGO.AddComponent<UIDocument>();
            c_lobbyMenu.visualTreeAsset = lobbyMenuVisuals;
            c_lobbyMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            c_lobbyMenu.rootVisualElement.SetEnabled(false);
            InitLobbyMenu();

            VisualTreeAsset settingsMenuVisuals = (VisualTreeAsset)Resources.Load(SETTINGS_MENU_UI_PATH);
            if (settingsMenuVisuals == null)
                throw new UnityException($"There is no visual tree asset for the settings menu in the Resources folder with the path {SETTINGS_MENU_UI_PATH}.");
            GameObject settingsMenuGO = new GameObject("SETTINGS_MENU");
            settingsMenuGO.transform.parent = c_canvas.transform;
            c_settingsMenu = settingsMenuGO.AddComponent<UIDocument>();
            c_settingsMenu.visualTreeAsset = settingsMenuVisuals;
            c_settingsMenu.panelSettings = m_data.GameData.UIData.PanelSettings;
            c_settingsMenu.rootVisualElement.SetEnabled(false);
            InitSettingsMenu();
            */
        }

        private void InitMainMenu()
        {
            menu_main.Document.rootVisualElement.Q<Button>(PLAY_WITH_AI_BUTTON).clicked += () =>
            {
                SwitchMenu(menu_ai);
            };

            menu_main.Document.rootVisualElement.Q<Button>(PLAY_WITH_PLAYER_BUTTON).clicked += () =>
            {
                SwitchMenu(menu_connecting);
            };

            menu_main.Document.rootVisualElement.Q<Button>(SETTINGS_BUTTON).clicked += () =>
            {
                //SwitchMenu(c_mainMenu, c_settingsMenu);
            };
        }

        private void InitAiMenu()
        {
            menu_ai.Document.rootVisualElement.Q<Button>(AI_PLAY_BUTTON).clicked += () =>
            {
                StartAiGame();
            };

            menu_ai.Document.rootVisualElement.Q<Button>(AI_RETURN_BUTTON).clicked += () =>
            {
                SwitchMenu(menu_main);
            };

            // ### NATIONS ### \\
            List<string> nation_choices = new();
            nation_choices.AddRange(new[] { m_data.GameData.AllyNation.DisplayName, m_data.GameData.AxisNation.DisplayName });
            DropdownField nationsDropdown = menu_ai.Document.rootVisualElement.Q<DropdownField>(AI_NATION);
            nationsDropdown.choices = nation_choices;
            nationsDropdown.index = 0;
            nation_selected = nation_choices[0];
            nationsDropdown.RegisterCallback<ChangeEvent<string>>(
                (val) => {
                    nation_selected = val.newValue;
                });

            // ### DIFFICULTIES ### \\
            List<string> difficulty_choices = new();
            foreach (AiBrainData difficulty in m_data.AiBrains)
                difficulty_choices.Add(difficulty.DisplayName);
            DropdownField difficultyDropdown = menu_ai.Document.rootVisualElement.Q<DropdownField>(AI_SELECTOR);
            difficultyDropdown.choices = difficulty_choices;
            difficultyDropdown.index = 0;
            difficulty_selected = difficulty_choices[0];
            difficultyDropdown.RegisterCallback<ChangeEvent<string>>(
                (val) => {
                    difficulty_selected = val.newValue;
                });

            // ### LEVELS ### \\
            ListView levels = menu_ai.Document.rootVisualElement.Q<ListView>(AI_LEVELS);
            foreach (LevelData level in m_data.Levels)
            {
                LevelSelector selector = new(level);
                selector.Button.clicked += () =>
                {
                    if (level_selected != null)
                        level_selected.SetSelection(false);
                    level_selected = selector;
                    level_selected.SetSelection(true);
                };
                if (level_selected == null)
                {
                    level_selected = selector;
                    level_selected.SetSelection(true);
                }
                levels.Q("unity-content-container").hierarchy.Add(selector);
            }
        }

        private void InitConnectingMenu()
        {
            NetworkHandler.Instance.Connection.ConnectedToMaster += () =>
            {
                SwitchMenu(menu_lobby);
            };

            NetworkHandler.Instance.Connection.FailedToConnect += () =>
            {
                SwitchMenu(menu_connectionFailed);
            };

            // TODO: Coroutine to wait for connection
            menu_connecting.OnShow += () =>
            {
                NetworkHandler.Instance.Connection.Connect();
            };
        }

        private void InitLobbyMenu()
        {
            menu_lobby.Document.rootVisualElement.Q(LOBBY_STARTING_GAME_MENU).style.display = DisplayStyle.None;
            // TODO: SHOW STARTING MATCH MESSAGE

            menu_lobby.Document.rootVisualElement.Q<Button>(LOBBY_CANCEL_BUTTON).clicked += () =>
            {
                NetworkHandler.Instance.Matchmacking.StopMatching();
                NetworkHandler.Instance.Connection.Disconnect();
                SwitchMenu(menu_main);
            };

            NetworkHandler.Instance.Matchmacking.OnFail += () =>
            {
                SwitchMenu(menu_connectionFailed);
            };

            NetworkHandler.Instance.Matchmacking.OnMatched += () =>
            {
                // TODO: TRANSITION
                menu_lobby.Document.rootVisualElement.Q(LOBBY_SEARCHING_MENU).SetEnabled(false);
                menu_lobby.Document.rootVisualElement.Q(LOBBY_STARTING_GAME_MENU).SetEnabled(true);
                // TODO: Level
                NetworkHandler.Instance.RoomHandler.StartGame(0);
            };

            NetworkHandler.Instance.RoomHandler.OnGameStarted += (level, other_player) =>
            {
                StartNetworkGame(other_player, level);
            };

            menu_lobby.OnShow += () =>
            {
                NetworkHandler.Instance.Matchmacking.FindMatch();
            };
        }

        private void InitFailedConnectionMenu()
        {
            menu_connectionFailed.Document.rootVisualElement.Q<Button>(FAILED_CONNECTION_BACK_BUTTON).clicked += () =>
            {
                SwitchMenu(menu_main);
            };
        }

        private void InitSettingsMenu()
        {
        }

        private Coroutine coroutine_menuSwitch;

        private void SwitchMenu(Menu next)
        {
            if (menu_active.Equals(next))
                return;
            if (coroutine_menuSwitch != null)
                return;
            coroutine_menuSwitch = StartCoroutine(SwitchMenuRoutine(next));
        }

        private IEnumerator SwitchMenuRoutine(Menu next)
        {
            menu_active.SetVisible(false);
            next.SetActive(true);
            yield return new WaitForSeconds(TRANSITION_BETWEEN_MENUS);
            menu_active.SetActive(false);
            next.SetVisible(true);
            menu_active = next;
            coroutine_menuSwitch = null;
        }

        // TODO: Set nation for player by selection
        private void StartAiGame()
        {
            if (level_selected == null)
                return;
            LevelData level = level_selected.Data;

            Nation nation;
            if (nation_selected.Equals(m_data.GameData.AllyNation.DisplayName))
                nation = m_data.GameData.AllyNation;
            else if (nation_selected.Equals(m_data.GameData.AxisNation.DisplayName))
                nation = m_data.GameData.AxisNation;
            else
                return;

            AiBrainData difficulty = null;
            foreach (AiBrainData ai in m_data.AiBrains)
            {
                if (difficulty_selected.Equals(ai.DisplayName))
                    difficulty = ai;
            }
            if (difficulty == null)
                return;

            GameBase.Config configuration = new(m_data.GameData, level);
            GameLocal.Config config = new(configuration, difficulty);
            GameLocal game = new(config);
            LoadGame(game, level);
        }

        private void StartNetworkGame(Photon.Realtime.Player network_player, byte level)
        {
            // TODO: Random map
            GameBase.Config configuration = new(m_data.GameData, m_data.Levels[level]);
            GameNetwork.Config config = new(network_player, configuration);
            GameNetwork game = new(config);
            LoadGame(game, m_data.Levels[0]);
        }

        private void LoadGame(GameBase game, LevelData level)
        {
            DontDestroyOnLoad(game.EMB.gameObject);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (game == null)
                    return;
                if (scene.name.Equals(level.SceneName))
                    game.Load();
            };
            SceneManager.LoadScene(level.SceneName);
        }

        private sealed class Menu
        {
            public delegate void EventHandler();
            public event EventHandler OnShow;

            private readonly UIDocument m_document;
            public UIDocument Document => m_document;

            public Menu(UIDocument document)
            {
                m_document = document;

                SetVisible(false);
                SetActive(false);
            }

            public void SetVisible(bool visible)
            {
                m_document.rootVisualElement.SetEnabled(visible);
                if (visible)
                    OnShow?.Invoke();
            }

            public void SetActive(bool active)
            {
                if (active)
                    m_document.sortingOrder = 1;
                else
                    m_document.sortingOrder = 0;
            }
        }
    }
}