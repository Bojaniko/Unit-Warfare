using UnityEngine;

namespace UnitWarfare.Units
{
    public abstract class UnitData : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

        [SerializeField] private string _displayName = "Unit";
        public string DisplayName => _displayName;

        [SerializeField] private Texture2D _displayTexture;
        public Texture2D DisplayTexture => _displayTexture;

        [SerializeField] private int m_manpower;
        public int Manpower => m_manpower;

        [SerializeField] private int _health;
        public int Health => _health;

        [SerializeField] private int _shield;
        public int Shield => _shield;

        [SerializeField] private int _attack;
        public int Attack => _attack;
    }
}