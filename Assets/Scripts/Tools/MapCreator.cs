using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;

using UnitWarfare.Units;
using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

namespace UnitWarfare.Tools
{
    public enum AddModes
    {
        SELECTION,
        MULTI_SELECTION,
        MULTI_FOLLOW,
        FOLLOW,
        ALL
    }

    public enum RemoveModes
    {
        SELECTION,
        MULTI_SELECTION,
        ALL
    }

    [EditorTool("Map Creator")]
    public class MapCreator : EditorWindow
    {
        private const string MAP_NAME = "MAP";
        private const string UI_PATH = "UI/MapCreator/map_creator";

        [SerializeField] private MapCreatorData _data;

        private GameObject c_map;
        private GameObject c_units;

        // ##### TERRITORY CREATION ##### \\

        private List<TerritoryIdentifier> _allTerritories;

        private LayerMask lm_territory;

        private AddModes _addMode = AddModes.SELECTION;
        private RemoveModes _removeMode = RemoveModes.SELECTION;

        private VisualElement _addOptions;
        private VisualElement _removeOptions;

        private Button _addButton;
        private Button _removeButton;
        private Button _paintButton;
        private Button _paintSelectionButton;

        private ScrollView _territoryTypes;

        private TerritoryTypeSelectionItem _selectedTerritoryType;

        private TerritoryCommand _territoryCommand = TerritoryCommand.NONE;

        public enum TerritoryCommand
        {
            ADD,
            REMOVE,
            PAINT,
            NONE
        }

        private void LoadTerritories()
        {
            _allTerritories = new();
            foreach (Transform t in c_map.transform)
            {
                TerritoryIdentifier ti = t.GetComponent<TerritoryIdentifier>();
                if (ti != null)
                    _allTerritories.Add(ti);
            }
        }

        private void InitTerritoryCommands()
        {
            _selectedTerritories = new();
            LoadTerritories();

            lm_territory = LayerMask.GetMask("Territory");

            List<Vector2> positions = new List<Vector2>(_data.TilePositions);
            positions.Add(Vector2.zero);
            _removePositions = positions.ToArray();
        }

        private void SetTerritoryCommand(TerritoryCommand command)
        {
            if (_territoryCommand.Equals(command))
                return;
            _territoryCommand = command;

            if (command.Equals(TerritoryCommand.ADD))
            {
                _addButton.AddToClassList("selected_command");
                _paintButton.RemoveFromClassList("selected_command");
                _removeButton.RemoveFromClassList("selected_command");

                _addOptions.style.display = DisplayStyle.Flex;
                _removeOptions.style.display = DisplayStyle.None;
                _paintSelectionButton.style.display = DisplayStyle.None;
            }
            else if (command.Equals(TerritoryCommand.REMOVE))
            {
                _removeButton.AddToClassList("selected_command");
                _paintButton.RemoveFromClassList("selected_command");
                _addButton.RemoveFromClassList("selected_command");

                _removeOptions.style.display = DisplayStyle.Flex;
                _addOptions.style.display = DisplayStyle.None;
                _paintSelectionButton.style.display = DisplayStyle.None;
            }
            else if (command.Equals(TerritoryCommand.PAINT))
            {
                _paintButton.AddToClassList("selected_command");
                _removeButton.RemoveFromClassList("selected_command");
                _addButton.RemoveFromClassList("selected_command");

                _paintSelectionButton.style.display = DisplayStyle.Flex;
                _removeOptions.style.display = DisplayStyle.None;
                _addOptions.style.display = DisplayStyle.None;
            }
        }

        private void BindTerritoryCommands()
        {
            _addOptions = rootVisualElement.Q<EnumField>("add_options");
            _removeOptions = rootVisualElement.Q<EnumField>("remove_options");

            _addOptions.RegisterCallback<ChangeEvent<System.Enum>>(
                evt => 
                {
                    _addMode = (AddModes)evt.newValue;
                });

            _removeOptions.RegisterCallback<ChangeEvent<System.Enum>>(
                evt =>
                {
                    _removeMode = (RemoveModes)evt.newValue;
                });


            _addButton = rootVisualElement.Q<Button>("territory_add");
            _removeButton = rootVisualElement.Q<Button>("territory_remove");
            _paintButton = rootVisualElement.Q<Button>("territory_paint");
            _paintSelectionButton = rootVisualElement.Q<Button>("territory_paint_selection");

            _addButton.RegisterCallback<MouseUpEvent>((evt) =>
            {
                SetTerritoryCommand(TerritoryCommand.ADD);
            });

            _removeButton.RegisterCallback<MouseUpEvent>((evt) =>
            {
                SetTerritoryCommand(TerritoryCommand.REMOVE);
            });

            _paintButton.RegisterCallback<MouseUpEvent>((evt) =>
            {
                SetTerritoryCommand(TerritoryCommand.PAINT);
            });

            _paintSelectionButton.RegisterCallback<MouseUpEvent>((evt) =>
            {
                if (_selectedTerritories.Count == 0)
                    return;
                foreach (TerritoryIdentifier ti in _selectedTerritories)
                    ti.SetTerritoryData(_selectedTerritoryType.TerritoryData);
            });

            SetTerritoryCommand(TerritoryCommand.ADD);

            _territoryTypes = rootVisualElement.Q<ScrollView>("territory_types");

            foreach (TerritoryData td in _data.TileData.TerritoryTypes)
            {
                TerritoryTypeSelectionItem item = new TerritoryTypeSelectionItem(td);
                item.Button.clicked += () =>
                {
                    foreach (VisualElement e in _territoryTypes.Children())
                        ((TerritoryTypeSelectionItem)e).SetActive(false);
                    item.SetActive(true);
                    _selectedTerritoryType = item;
                };
                _territoryTypes.Add(item);
            }

            _selectedTerritoryType = (TerritoryTypeSelectionItem)_territoryTypes.ElementAt(0);
            _selectedTerritoryType.SetActive(true);
        }

        private Vector2[] _removePositions;

        private void DrawRemoveHandlesForTiles(SceneView view)
        {
            switch (_removeMode)
            {
                case RemoveModes.SELECTION:
                    if (_selectedTerritories.Count == 0)
                        break;
                    DrawRemoveHandlesForTile(_selectedTerritories[0], view);
                    break;

                case RemoveModes.MULTI_SELECTION:
                    if (_selectedTerritories.Count == 0)
                        break;
                    foreach (TerritoryIdentifier ti in _selectedTerritories.ToArray())
                        DrawRemoveHandlesForTile(ti, view);
                    break;

                case RemoveModes.ALL:
                    foreach (TerritoryIdentifier ti in _allTerritories.ToArray())
                        DrawRemoveHandlesForTile(ti, view);
                    break;
            }
        }

        private void DrawRemoveHandlesForTile(TerritoryIdentifier tile, SceneView view)
        {
            if (tile == null)
                return;

            Handles.color = Color.red;
            foreach (Vector2 pos in _removePositions)
            {
                Vector3 btnPos = new Vector3(tile.transform.position.x + (pos.x * _mapScale),
                    tile.transform.position.y,
                    tile.transform.position.z + (pos.y * _mapScale));

                if (!TileExists(btnPos))
                    continue;

                if (Handles.Button(btnPos,
                    view.camera.transform.rotation,
                    0.15f, 0.15f,
                    Handles.DotHandleCap))
                {
                    RemoveTile(GetTileAtPosition(btnPos));
                }
            }
        }

        private void RemoveTile(TerritoryIdentifier ti)
        {
            _allTerritories.Remove(ti);
            Undo.RegisterCompleteObjectUndo(ti.gameObject, "Tile removed");
            Undo.DestroyObjectImmediate(ti.gameObject);
        }

        private void DrawAddHandlesForTiles(SceneView view)
        {
            switch (_addMode)
            {
                case AddModes.SELECTION:
                    if (_selectedTerritories.Count == 0)
                        break;
                    DrawAddHandlesForTile(_selectedTerritories[_selectedTerritories.Count - 1], view);
                    break;

                case AddModes.MULTI_SELECTION:
                    if (_selectedTerritories.Count == 0)
                        break;
                    foreach (TerritoryIdentifier ti in _selectedTerritories)
                        DrawAddHandlesForTile(ti, view);
                    break;

                case AddModes.MULTI_FOLLOW:
                    if (_selectedTerritories.Count == 0)
                        break;
                    foreach (TerritoryIdentifier ti in _selectedTerritories.ToArray())
                        DrawAddHandlesForTile(ti, view);
                    break;

                case AddModes.FOLLOW:
                    if (_selectedTerritories.Count == 0)
                        break;
                    DrawAddHandlesForTile(_selectedTerritories[0], view);
                    break;

                case AddModes.ALL:
                    foreach (TerritoryIdentifier ti in _allTerritories.ToArray())
                        DrawAddHandlesForTile(ti, view);
                    break;
            }
        }

        private void DrawAddHandlesForTile(TerritoryIdentifier tile, SceneView view)
        {
            Handles.color = Color.green;
            foreach (Vector2 pos in _data.TilePositions)
            {
                Vector3 btnPos = new Vector3(tile.transform.position.x + (pos.x * _mapScale),
                    tile.transform.position.y,
                    tile.transform.position.z + (pos.y * _mapScale));

                if (TileExists(btnPos))
                    continue;

                if (Handles.Button(btnPos,
                    view.camera.transform.rotation,
                    0.1f, 0.1f,
                    Handles.DotHandleCap))
                {
                    CreateNewTile(btnPos);
                }
            }
        }

        private byte GetCurrentTileId()
        {
            byte current = 0;
            for (byte i = 0; i <= byte.MaxValue; i++)
            {
                bool available = true;
                foreach (TerritoryIdentifier ti in _allTerritories)
                {
                    if (ti.ID.Equals(current))
                    {
                        available = false;
                        break;
                    }
                }
                if (available)
                    return current;
            }
            throw new System.ArithmeticException("Can't have more than 256 tiles per level.");
        }

        private void CreateNewTile(Vector3 position)
        {
            byte current_id = 0;

            try
            {
                current_id = GetCurrentTileId();
            }
            catch
            {
                Debug.LogError("Max territories per level is 256.");
            }

            TerritoryIdentifier ti = TerritoryFactory.CreateTerritoryIdentifier(_data.TileData, _selectedTerritoryType.TerritoryData, position, GetCurrentTileId());

            Undo.RegisterCreatedObjectUndo(ti.gameObject, "Tile created");

            _allTerritories.Add(ti);

            if (_addMode.Equals(AddModes.FOLLOW))
            {
                Selection.objects = new GameObject[] { ti.gameObject };
                _selectedTerritories.Clear();
                _selectedTerritories.Add(ti);
            }
            if (_addMode.Equals(AddModes.MULTI_FOLLOW))
            {
                _selectedTerritories.Add(ti);
                Selection.objects = _selectedTerritories.ToArray();
            }
        }

        private bool TileExists(Vector3 position) =>
            Physics.CheckSphere(position, _data.TileData.TileRadius * 0.5f * _mapScale, lm_territory);

        private TerritoryIdentifier GetTileAtPosition(Vector3 position)
        {
            Collider[] hits = Physics.OverlapSphere(position, _data.TileData.TileRadius * 0.5f * _mapScale, lm_territory);
            if (hits[0] != null)
                return hits[0].transform.parent.GetComponent<TerritoryIdentifier>();
            return null;
        }

        private void DrawTerritoryInfo(TerritoryIdentifier territory)
        {
            GUIStyle style = new();
            style.alignment = TextAnchor.MiddleCenter;

            if (territory.Owner.Equals(PlayerIdentifiers.NEUTRAL))
                style.normal.textColor = Color.white;
            else if (territory.Owner.Equals(PlayerIdentifiers.PLAYER_ONE))
                style.normal.textColor = Color.green;
            else if (territory.Owner.Equals(PlayerIdentifiers.PLAYER_TWO))
                style.normal.textColor = Color.red;

            Handles.Label(territory.transform.position, territory.Owner.ToString(), style);
        }

        private float _mapScale;

        /*private void OnInspectorUpdate()
        {
            if (c_map.transform.localScale.x != _mapScale)
            {
                _mapScale = c_map.transform.localScale.x;
                c_map.transform.localScale = Vector3.one * _mapScale;
            }
        }*/

        // ##### UNITS ##### \\

        private List<UnitIdentifier> _allUnits;
        private void LoadUnits()
        {
            _allUnits = new();

            foreach (Transform t in c_units.transform)
            {
                UnitIdentifier ui = t.GetComponent<UnitIdentifier>();
                if (ui != null)
                    _allUnits.Add(ui);
            }
        }

        private void InitUnitCommands()
        {
            LoadUnits();
        }

        private ScrollView _unitTypes;

        private UnitData _selectedUnitData;
        private void BindUnitsCommands()
        {
            _unitTypes = rootVisualElement.Q<ScrollView>("unit_types");
            foreach (UnitData data in _data.UnitsData.AllData)
            {
                if (data == null)
                    continue;
                UnitTypeSelectionItem item = new UnitTypeSelectionItem(data);
                item.Button.clicked += () =>
                {
                    foreach (UnitTypeSelectionItem i in _unitTypes.Children())
                        i.SetActive(false);
                    item.SetActive(true);
                    _selectedUnitData = item.UnitData;
                };
                _unitTypes.Add(item);
            }
            UnitTypeSelectionItem first = (UnitTypeSelectionItem)_unitTypes[0];
            _selectedUnitData = first.UnitData;
            first.SetActive(true);

            rootVisualElement.Q<Button>("unit_create").clicked += () =>
            {
                if (_selectedTerritories.Count == 0)
                    return;
                UnitIdentifier ui = UnitFactory.GenerateUnitIdentifier(_data.ActiveUnitPrefab, _selectedTerritories[0], _selectedUnitData, c_units.transform);
                _allUnits.Add(ui);

                Undo.RegisterCreatedObjectUndo(ui.gameObject, ui.Data.DisplayName);

                SetUnitColor(ui, _selectedTerritories[0].Owner);
            };

            rootVisualElement.Q<Button>("unit_update").clicked += () =>
            {
                if (_selectedUnit != null)
                    _selectedUnit.Data = _selectedUnitData;
            };

            rootVisualElement.Q<Button>("unit_remove").clicked += () =>
                {
                    if (_selectedUnit != null)
                    {
                        _allUnits.Remove(_selectedUnit);
                        DestroyImmediate(_selectedUnit.gameObject);
                    }
                };
        }

        private void SetUnitColor(UnitIdentifier unit, PlayerIdentifiers owner)
        {
            switch (owner)
            {
                case PlayerIdentifiers.PLAYER_ONE:
                    unit.transform.GetChild(0).GetComponent<MeshRenderer>().material = _data.PlayerUnitMaterial;
                    break;

                case PlayerIdentifiers.PLAYER_TWO:
                    unit.transform.GetChild(0).GetComponent<MeshRenderer>().material = _data.OtherPlayerUnitMaterial;
                    break;

                case PlayerIdentifiers.NEUTRAL:
                    unit.transform.GetChild(0).GetComponent<MeshRenderer>().material = _data.NeutralUnitMaterial;
                    break;
            }
        }

        private void DrawUnitInfo(UnitIdentifier unit)
        {
            GUIStyle style = new();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            Handles.Label(unit.transform.position, unit.Data.DisplayName, style);
        }

        private void DrawUnitsInfo()
        {
            if (_selectedTerritories.Count > 0)
                DrawTerritoryInfo(_selectedTerritories[0]);
            foreach (UnitIdentifier ui in _allUnits)
            {
                DrawUnitInfo(ui);
            }
        }

        private void DisposeUnitMeshPreviews()
        {
            if (_unitTypes == null)
                return;
            foreach (UnitTypeSelectionItem item in _unitTypes.Children())
                item.Dispose();
        }

        // ##### OWNERSHIP ##### \\

        private PlayerIdentifiers _selectedTerritoryOwner;

        private void DrawOwnershipInfo()
        {
            foreach (TerritoryIdentifier ti in _allTerritories)
            {
                DrawTerritoryInfo(ti);
            }
        }

        private void BindOwnershipCommands()
        {
            rootVisualElement.Q<EnumField>("owner_options").RegisterCallback<ChangeEvent<System.Enum>>(
                evt =>
                {
                    _selectedTerritoryOwner = (PlayerIdentifiers)evt.newValue;
                });

            rootVisualElement.Q<Button>("owner_change_button").RegisterCallback<MouseUpEvent>(
                evt =>
                {
                    if (_selectedTerritories.Count == 0)
                        return;
                    foreach (TerritoryIdentifier ti in _selectedTerritories)
                    {
                        ti.Owner = _selectedTerritoryOwner;
                        Undo.RegisterCompleteObjectUndo(ti, "Changed ownership.");
                        foreach (UnitIdentifier ui in _allUnits)
                        {
                            if (ui.StartingTerritory.Equals(ti))
                            {
                                SetUnitColor(ui, ti.Owner);
                                break;
                            }
                        }
                    }
                });
        }

        // ##### SELECTION ##### \\

        private void OnSelectionChange()
        {
            if (_data == null)
            {
                Debug.LogError("Data for Map Creator not assigned!");
                return;
            }

            GameObject[] selection = Selection.GetFiltered<GameObject>(SelectionMode.Unfiltered);

            HandleSelectedTile(selection);
            HandleSelectedUnit(selection);
        }

        private UnitIdentifier _selectedUnit;
        private void HandleSelectedUnit(GameObject[] selection)
        {
            foreach (GameObject go in selection)
            {
                _selectedUnit = go.GetComponent<UnitIdentifier>();
                if (_selectedUnit == null && go.transform.parent != null)
                    _selectedUnit = go.transform.parent.GetComponent<UnitIdentifier>();
                if (_selectedUnit != null)
                    return;
            }
        }

        private List<TerritoryIdentifier> _selectedTerritories;
        private void HandleSelectedTile(GameObject[] selection)
        {
            _selectedTerritories.Clear();

            foreach (GameObject go in selection)
            {
                TerritoryIdentifier ti = go.GetComponent<TerritoryIdentifier>();
                if (ti == null && go.transform.parent != null)
                    ti = go.transform.parent.GetComponent<TerritoryIdentifier>();
                if (ti != null)
                    _selectedTerritories.Add(ti);
            }

            if (_selectedTerritories.Count > 0)
                UnityEditor.Tools.current = Tool.Custom;
        }

        // ##### SCENE GUI ##### \\

        private void OnFocus()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnBecameInvisible()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            if (Application.isPlaying)
                return;

            if (_currentMenu.Equals(CreatorMenu.TERRITORY))
            {
                if (_territoryCommand.Equals(TerritoryCommand.ADD))
                    DrawAddHandlesForTiles(view);
                if (_territoryCommand.Equals(TerritoryCommand.REMOVE))
                    DrawRemoveHandlesForTiles(view);
            }
            else if (_currentMenu.Equals(CreatorMenu.OWNERSHIP))
            {
                DrawOwnershipInfo();
            }
            else if (_currentMenu.Equals(CreatorMenu.UNITS))
            {
                DrawUnitsInfo();
            }
        }

        // ##### INITIALIZATION ##### \\

        private void Awake()
        {
            c_map = GameObject.Find(MAP_NAME);
            if (c_map == null)
            {
                Debug.LogError("First create a 'MAP' game object and attach it a TerritoryManager script!");
                Close();
                return;
            }

            c_units = GameObject.Find("UNITS");
            if (c_units == null)
            {
                Debug.LogError("First create a 'UNITS' game object!");
                Close();
                return;
            }

            _mapScale = c_map.transform.localScale.x;

            InitTerritoryCommands();

            InitUnitCommands();
        }

        private void CreateGUI()
        {
            Object o = Resources.Load(UI_PATH);
            VisualTreeAsset asset = (VisualTreeAsset)o;
            asset.CloneTree(rootVisualElement);

            AttachSavedDataObject();

            LoadMenus();

            BindUI();
        }

        private void BindUI()
        {
            BindTerritoryCommands();

            BindOwnershipCommands();

            BindUnitsCommands();
        }

        private void OnDisable()
        {
            DisposeUnitMeshPreviews();
        }

        // ##### SAVE DATA FILE ##### \\

        private void SaveSelectedData()
        {
            if (_data != null)
                EditorPrefs.SetString("MapCreator Data Path", AssetDatabase.GetAssetPath(_data.GetInstanceID()));
        }

        private void AttachSavedDataObject()
        {
            string dataPath = EditorPrefs.GetString("MapCreator Data Path");
            if (!string.IsNullOrEmpty(dataPath))
            {
                _data = (MapCreatorData)AssetDatabase.LoadAssetAtPath(dataPath, typeof(MapCreatorData));
                if (_data == null)
                    _data = (MapCreatorData)AssetDatabase.FindAssets("t: MapCreatorData").GetValue(0);
                rootVisualElement.Q<ObjectField>("data_field").value = _data;
            }
        }

        private void OnValidate()
        {
            SaveSelectedData();
        }

        // ##### EDITOR MENU COMMANDS ##### \\

        [MenuItem("Unit Warfare/Map Creator")]
        public static void OpenMapCreator()
        {
            var window = GetWindow<MapCreator>();
            window.titleContent = new GUIContent("Map Creator");
        }

        [MenuItem("Unit Warfare/Reload Tile IDs")]
        public static void ReloadIDs()
        {
            GameObject map = GameObject.Find(MAP_NAME);
            if (map == null)
            {
                Debug.LogError($"There is no gameobject in the scene named {MAP_NAME} that parents all tiles. Please create a new map 'Unit Warfare/New Map' or check if your level is valid.");
                return;
            }
            byte currentId = 0;
            foreach (Transform t in map.transform)
            {
                TerritoryIdentifier ti = t.GetComponent<TerritoryIdentifier>();
                if (ti != null)
                {
                    try
                    {
                        ti.SetID(currentId);
                        currentId++;
                    }
                    catch
                    {
                        Debug.LogError("Max tiles for map is 256.");
                        DestroyImmediate(ti);
                    }
                }
            }
            Undo.RegisterFullObjectHierarchyUndo(map.transform, "Reloaded territory ids.");
        }

        // ##### MENUS ##### \\

        public enum CreatorMenu
        {
            TERRITORY,
            UNITS,
            OWNERSHIP
        }

        private CreatorMenu _currentMenu;

        private List<CreatorMenuItem> _menuItems;

        private void LoadMenus()
        {
            _menuItems = new();

            _menuItems.Add(new(rootVisualElement.Q("territories_container"), rootVisualElement.Q<Button>("option_territory"), CreatorMenu.TERRITORY));
            _menuItems.Add(new(rootVisualElement.Q("units_container"), rootVisualElement.Q<Button>("option_units"), CreatorMenu.UNITS));
            _menuItems.Add(new(rootVisualElement.Q("ownership_container"), rootVisualElement.Q<Button>("option_ownership"), CreatorMenu.OWNERSHIP));

            foreach (CreatorMenuItem cmi in _menuItems)
            {
                cmi.OnShowMenu += (i) =>
                {
                    _currentMenu = i.Menu;
                    foreach (CreatorMenuItem cmi in _menuItems)
                    {
                        if (cmi.Equals(i))
                            continue;
                        cmi.HideMenu();
                    }
                };
            }

            _menuItems[0].ShowMenu();
        }

        private sealed class CreatorMenuItem
        {
            public delegate void CreatorMenuItemEventHandler(CreatorMenuItem item);
            public event CreatorMenuItemEventHandler OnShowMenu;

            private readonly Button _button;

            private readonly VisualElement _element;

            private readonly CreatorMenu _menu;
            public CreatorMenu Menu => _menu;

            public CreatorMenuItem(VisualElement element, Button button, CreatorMenu menu)
            {
                _element = element;
                _menu = menu;
                _button = button;
                _button.clicked += () => { ShowMenu(); };
            }

            public void ShowMenu()
            {
                _element.style.display = DisplayStyle.Flex;
                _button.AddToClassList("selected_command");
                OnShowMenu?.Invoke(this);
            }

            public void HideMenu()
            {
                _element.style.display = DisplayStyle.None;
                _button.RemoveFromClassList("selected_command");
            }
        }
    }
}