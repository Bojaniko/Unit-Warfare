using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace UnitWarfare.Units
{
    [CustomEditor(typeof(UnitCombinations))]
    public class UnitCombinationsEditor : Editor
    {
        private const string path = "UI/unit_combinations";

        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset vta = (VisualTreeAsset)Resources.Load(path);
            VisualElement root = new();
            vta.CloneTree(root);

            return root;
        }
    }
}