using UnityEngine;

namespace UnitWarfare.Units
{
    public class ActiveUnitStarter : MonoBehaviour
    {
        [SerializeField] private GameObject m_muzzleFlash;
        public GameObject MuzzleFlash => m_muzzleFlash;

        [SerializeField] private Animator m_animator;
        public Animator Animator => m_animator;
    }
}