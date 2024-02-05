using UnityEngine;
using UnityEngine.UIElements;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UnitDisplay : MonoBehaviour, IUserInterfaceHandler
    {
        public record UnitDisplayData(Texture2D Image, string Name, int Attack, int Shield, int Health);

        private UIDocument _document;

        private VisualElement ve_image;
        private Label l_name;
        private Label l_attack;
        private Label l_shield;
        private Label l_health;

        private StyleBackground _defaultImage;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();

            ve_image = _document.rootVisualElement.Q<VisualElement>("unit_image");
            _defaultImage = ve_image.style.backgroundImage;

            l_name = _document.rootVisualElement.Q<Label>("unit_name");
            l_attack = _document.rootVisualElement.Q<Label>("attack_val");
            l_shield = _document.rootVisualElement.Q<Label>("shield_val");
            l_health = _document.rootVisualElement.Q<Label>("health_val");

            HideDisplay();
        }

        public void DisplayUnit(UnitDisplayData unit_data)
        {
            if (unit_data.Image != null)
                ve_image.style.backgroundImage = unit_data.Image;
            else
                ve_image.style.backgroundImage = _defaultImage;

            l_name.text = unit_data.Name;
            l_attack.text = $"{unit_data.Attack}";
            l_shield.text = $"{unit_data.Shield}";
            l_health.text = $"{unit_data.Health}";

            _document.rootVisualElement.SetEnabled(true);
        }

        public void HideDisplay()
        {
            _document.rootVisualElement.SetEnabled(false);
        }
    }
}