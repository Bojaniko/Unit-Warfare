using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace UnitWarfare.Units
{
    [CustomPropertyDrawer(typeof(UnitsData.DataContainer))]
    public class DataContainerPropertyDrawer : PropertyDrawer
    {
        private const string PROPERTY_NAME_TYPE = "Type";
        private const string PROPERTY_NAME_DATA = "Data";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            string typeName = property.FindPropertyRelative(PROPERTY_NAME_TYPE).stringValue;
            System.Type type = typeof(UnitsData).Assembly.GetType(typeName);
            ObjectField of = new();
            of.label = type.Name;
            of.objectType = type;
            of.bindingPath = PROPERTY_NAME_DATA;

            VisualElement root = new();
            root.name = "container-root";
            root.Add(of);
            return root;
        }
    }
}