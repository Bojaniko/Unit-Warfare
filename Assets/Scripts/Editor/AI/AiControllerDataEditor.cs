using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnitWarfare.AI
{
    [CustomEditor(typeof(AiBrainData))]
    public class AiControllerDataEditor : Editor
    {
        private const string path = "UI/AI/ai_brain_data";

        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset vta = (VisualTreeAsset)Resources.Load(path);
            VisualElement root = new();
            vta.CloneTree(root);

            return root;
        }
    }
}