using UnityEngine;
using UnityEngine.UIElements;

using UnitWarfare.Core;

namespace UnitWarfare.Game.UI
{
    public class LevelSelector : VisualElement
    {
        private readonly static VisualTreeAsset asset_visual;
        static LevelSelector()
        {
            asset_visual = (VisualTreeAsset)Resources.Load(VISUAL_ASSET_PATH);
        }

        private const string VISUAL_ASSET_PATH = "UI/Main/level_selection";
        private const string NAME_LABEL = "name";
        private const string NAME_IMAGE = "image";
        private const string NAME_IMAGE_SELECTED = "image_selected";

        private readonly Button m_button;
        public Button Button => m_button;

        private readonly LevelData m_data;
        public LevelData Data => m_data;

        private readonly VisualElement c_selected;

        public LevelSelector(LevelData data)
        {
            if (asset_visual == null)
                throw new UnityException($"There is no visual tree asset at path '{VISUAL_ASSET_PATH}' in Resources.");
            asset_visual.CloneTree(this);
            m_button = this.Q<Button>();
            m_data = data;

            this.Q<Label>(NAME_LABEL).text = data.DisplayName;
            StyleBackground background = new(data.Icon);
            VisualElement image = this.Q<VisualElement>(NAME_IMAGE);
            image.style.backgroundImage = background;

            c_selected = this.Q(NAME_IMAGE_SELECTED);
            c_selected.style.display = DisplayStyle.None;
        }

        public void SetSelection(bool selected)
        {
            if (selected)
                c_selected.style.display = DisplayStyle.Flex;
            else
                c_selected.style.display = DisplayStyle.None;
        }
    }
}
