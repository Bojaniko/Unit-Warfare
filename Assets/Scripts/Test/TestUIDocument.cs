using UnityEngine;
using UnityEngine.UIElements;

namespace UnitWarfare.Test
{
    public class TestUIDocument : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        public void ShowHide()
        {
            _document.rootVisualElement.SetEnabled(!_document.rootVisualElement.enabledSelf);
        }
    }
}