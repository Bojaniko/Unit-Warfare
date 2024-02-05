using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

using UnitWarfare.Units;

namespace UnitWarfare.Tools
{
    public class UnitTypeSelectionItem : VisualElement, System.IDisposable
    {
        private const string ui_path = "UI/MapCreator/selection_item_unit";

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UnitTypeSelectionItem> { }

        public UnitTypeSelectionItem()
        {
            Object o = Resources.Load(ui_path);
            VisualTreeAsset asset = o as VisualTreeAsset;
            VisualElement item = new();
            asset.CloneTree(item);
            hierarchy.Add(item);
        }

        private readonly UnitData _data;
        public UnitData UnitData => _data;

        private readonly VisualElement _selected;

        private readonly Button _button;
        public Button Button => _button;

        private readonly IMGUIContainer _meshContainer;
        private readonly MeshPreview _meshPreview;

        public UnitTypeSelectionItem(UnitData unit_data)
        {
            _data = unit_data;
            if (_data == null)
                return;

            Object o = Resources.Load(ui_path);
            VisualTreeAsset asset = o as VisualTreeAsset;
            VisualElement item = new();
            asset.CloneTree(item);
            hierarchy.Add(item);

            if (_data.Prefab != null)
            {
                Mesh mesh = null;
                foreach (Transform t in _data.Prefab.transform)
                {
                    if (t.CompareTag("UnitModel"))
                    {
                        MeshFilter filter;
                        if (t.TryGetComponent<MeshFilter>(out filter))
                        {
                            mesh = filter.sharedMesh;
                        }
                        SkinnedMeshRenderer smr;
                        if (mesh == null && t.TryGetComponent<SkinnedMeshRenderer>(out smr))
                        {
                            mesh = smr.sharedMesh;
                        }
                    }
                }

                if (mesh != null)
                {
                    _meshPreview = new(mesh);
                    _meshContainer = item.Q<IMGUIContainer>("item_mesh");
                    _meshContainer.onGUIHandler += () =>
                    {
                        _meshPreview.OnPreviewGUI(_meshContainer.contentRect, new GUIStyle());
                    };
                }
            }

            item.Q<Label>("item_label").text = _data.DisplayName;
            item.Q<Label>("item_health").text = $"Health: {unit_data.Health}";
            item.Q<Label>("item_shield").text = $"Shield: {unit_data.Shield}";
            item.Q<Label>("item_attack").text = $"Attack: {unit_data.Attack}";

            _button = item.Q<Button>();

            _selected = item.Q("item_image_selection");
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            if (active)
                _selected.style.display = DisplayStyle.Flex;
            else
                _selected.style.display = DisplayStyle.None;
        }

        public void Dispose()
        {
            if (_meshContainer != null)
                _meshContainer.onGUIHandler = null;
            if (_meshPreview != null)
                _meshPreview.Dispose();
        }
    }
}