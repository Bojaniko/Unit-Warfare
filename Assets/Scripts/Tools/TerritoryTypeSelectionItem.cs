using UnityEngine;
using UnityEngine.UIElements;

using UnitWarfare.Territories;

namespace UnitWarfare.Tools
{
    public class TerritoryTypeSelectionItem : VisualElement
    {
        private const string ui_path = "UI/MapCreator/selection_item";

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TerritoryTypeSelectionItem>
        {
        }

        public TerritoryTypeSelectionItem()
        {
            Object o = Resources.Load(ui_path);
            VisualTreeAsset asset = o as VisualTreeAsset;
            VisualElement item = new();
            asset.CloneTree(item);
            hierarchy.Add(item);
        }

        private readonly TerritoryData _data;
        public TerritoryData TerritoryData => _data;

        private readonly VisualElement _selected;

        private readonly Button _button;
        public Button Button => _button;

        public TerritoryTypeSelectionItem(TerritoryData territory_data)
        {
            _data = territory_data;

            Object o = Resources.Load(ui_path);
            VisualTreeAsset asset = o as VisualTreeAsset;
            VisualElement item = new();
            asset.CloneTree(item);
            hierarchy.Add(item);

            item.Q("item_image").style.backgroundImage = _data.MainImage;
            item.Q<Label>("item_label").text = _data.Name;

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
    }
}