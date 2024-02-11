using UnityEngine;

using UnitWarfare.Units;

namespace UnitWarfare.Players
{
    [CreateAssetMenu(menuName = "Game/Nation")]
    public class Nation : ScriptableObject
    {
        [SerializeField] private string m_displayName;
        public string DisplayName => m_displayName;

        [SerializeField] private Texture m_icon;
        public Texture Icon => m_icon;

        [SerializeField] private UnitsData m_units;
        public UnitsData Units => m_units;
    }
}