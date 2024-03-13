using UnityEngine;

using UnitWarfare.Core.Global;

namespace UnitWarfare.Core
{
	[CreateAssetMenu(menuName = "Game/Level")]
	public class LevelData : ScriptableObject
	{
		[SerializeField] private int m_maxManpower;
		public int MaxManpower => m_maxManpower;

		[SerializeField] private float m_maxRoundDuration;
		public float MaxRoundDuration => m_maxRoundDuration;

		[SerializeField] private string m_scene;
		public string SceneName => m_scene;
		
		[SerializeField] private string m_displayName;
		public string DisplayName => m_displayName;

		[SerializeField] private Texture2D m_icon;
		public Texture2D Icon => m_icon;

		[SerializeField] private LevelTheme m_theme;
		public LevelTheme Theme => m_theme;
	}
}