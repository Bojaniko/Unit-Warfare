using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

using UnitWarfare.Units;

namespace UnitWarfare.UI
{
    [CustomPropertyDrawer(typeof(UnitCombinations.Combination))]
    public class UnitCombinationPropertyDrawer : PropertyDrawer
    {
        /*private static System.Type[] _unitTypes = GetUnitTypes();
        private static System.Type[] GetUnitTypes()
        {
            List<System.Type> types = new();
            foreach (System.Type t in typeof(IUnit).Assembly.GetTypes())
            {
                if (t.GetInterface("IUnit") == null)
                    continue;
                if (t.IsAbstract)
                    continue;
                if (t.IsInterface)
                    continue;
                types.Add(t);
            }
            return types.ToArray();
        }*/

        private const string path = "UI/unit_combinations_selector";

        private string[] _typeNames;

        private void GenerateTypeNames()
        {
            List<string> names = new();
            foreach (System.Type t in UnitCombinations.UnitTypes)
                names.Add(t.Name);
            _typeNames = names.ToArray();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (UnitCombinations.UnitTypes.Length == 0)
                return new Label("There are no Unit Types!");

            GenerateTypeNames();

            VisualTreeAsset vta = (VisualTreeAsset)Resources.Load(path);
            VisualElement root = new();
            vta.CloneTree(root);

            DropdownField unit_one = root.Q<DropdownField>("unit_one_selector");
            DropdownField unit_two = root.Q<DropdownField>("unit_two_selector");
            DropdownField result = root.Q<DropdownField>("unit_result");

            unit_one.choices = new List<string>(_typeNames);
            unit_two.choices = new List<string>(_typeNames);
            result.choices = new List<string>(_typeNames);

            unit_one.value = property.FindPropertyRelative("UnitOne").stringValue;
            unit_two.value = property.FindPropertyRelative("UnitTwo").stringValue;
            result.value = property.FindPropertyRelative("Result").stringValue;

            unit_one.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                foreach (string tn in _typeNames)
                {
                    if (tn.Equals(evt.newValue))
                    {
                        property.FindPropertyRelative("UnitOne").stringValue = tn;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
            });

            unit_two.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                foreach (string tn in _typeNames)
                {
                    if (tn.Equals(evt.newValue))
                    {
                        property.FindPropertyRelative("UnitTwo").stringValue = tn;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
            });

            result.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                foreach (string tn in _typeNames)
                {
                    if (tn.Equals(evt.newValue))
                    {
                        property.FindPropertyRelative("Result").stringValue = tn;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
            });

            return root;
        }
    }
}