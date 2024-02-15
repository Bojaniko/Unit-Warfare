using UnityEngine;
using UnityEditor;

using UnitWarfare.Units;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Test
{
    public class UnitTest : MonoBehaviour
    {
        [SerializeField] private GameObject _unit;
        [SerializeField] private GameObject _target;
        [SerializeField] private ActiveCommandOrder _command;

        public void TestUnit()
        {
            if (_unit == null)
            {
                Debug.LogError("Soldier not set!");
                return;
            }
            if (_unit.GetComponent<IUnit>() == null)
            {
                Debug.LogError("Selected Soldier does not have a SolderUnit script attached.");
                return;
            }

            if (_target == null)
            {
                Debug.LogError("Target not set!");
                return;
            }

            /*if (_command.Equals(CommandOrder.MOVE))
            {
                UnitCommand command = new UnitCommand(_command, new(_target.transform.position));
                _unit.GetComponent<IUnit>().StartCommand(command);
            }*/
        }
    }

    [CustomEditor(typeof(UnitTest))]
    public class UnitTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Test!"))
            {
                UnitTest test = (UnitTest)serializedObject.targetObject;
                test.TestUnit();
            }
        }
    }
}