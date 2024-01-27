using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    [CustomPropertyDrawer(typeof(BrainFeature))]
    public class BrainFeaturePropertyDrawer : PropertyDrawer
    {
        private const string path = "UI/AI/ai_brain_feature";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualTreeAsset vta = (VisualTreeAsset)Resources.Load(path);
            VisualElement root = new();
            vta.CloneTree(root);

            SliderInt feature = root.Q<SliderInt>("feature");
            feature.label = System.Enum.GetValues(typeof(AiBrainFeature)).GetValue(property.FindPropertyRelative("_feature").intValue).ToString();

            return root;
        }
    }
}