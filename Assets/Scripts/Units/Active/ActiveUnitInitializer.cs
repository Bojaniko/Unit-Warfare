using UnityEngine;

namespace UnitWarfare.Units
{
    public class ActiveUnitInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject _muzzleFlash;
        public GameObject MuzzleFlash => _muzzleFlash;
    }
}