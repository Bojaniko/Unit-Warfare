using UnityEngine;
using UnityEngine.UIElements;

namespace UnitWarfare.UI
{
    [CreateAssetMenu(menuName = "UI/Data")]
    public class UIData : ScriptableObject
    {
        [SerializeField] private PanelSettings m_panelSettings;
        public PanelSettings PanelSettings => m_panelSettings;
    }
}