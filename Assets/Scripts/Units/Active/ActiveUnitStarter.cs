using UnityEngine;

namespace UnitWarfare.Units
{
    public class ActiveUnitStarter : MonoBehaviour
    {
        [SerializeField] private GameObject _muzzleFlash;
        public GameObject MuzzleFlash => _muzzleFlash;
    }
}