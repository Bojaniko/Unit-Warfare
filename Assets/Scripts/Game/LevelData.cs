using UnityEngine;

namespace UnitWarfare.Game
{
    [CreateAssetMenu(menuName = "Game/Level")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string m_scene;
        public string SceneName => m_scene;

        [SerializeField] private string m_displayName;
        public string DisplayName => m_displayName;

        [SerializeField] private Texture2D m_icon;
        public Texture2D Icon => m_icon;
    }
}