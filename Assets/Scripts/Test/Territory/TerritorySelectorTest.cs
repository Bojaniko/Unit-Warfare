using UnityEngine;
using UnityEditor;

using UnitWarfare.Territories;

namespace UnitWarfare.Test
{
    public class TerritorySelectorTest : MonoBehaviour
    {
        private Territory _territory;

        private void Awake()
        {
            _territory = GetComponent<Territory>();
        }

        public void Select()
        {
            if (_territory.Selected)
                _territory.DisableSelection();
            else
                _territory.EnableSelection(Territory.SelectionType.ACTIVE);
        }

        [CustomEditor(typeof(TerritorySelectorTest))]
        private class TerritorySelectorTestEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                if (GUILayout.Button("Selected"))
                     ((TerritorySelectorTest)target).Select();
            }
        }
    }
}