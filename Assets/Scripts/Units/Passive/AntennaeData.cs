using UnityEngine;

namespace UnitWarfare.Units
{
    [CreateAssetMenu(menuName = "Units/Passive/Antennae")]
    public class AntennaeData : UnitData
    {
        [SerializeField] private SoldierData m_recruitData;
        public SoldierData RecruitData => m_recruitData;

        [SerializeField] private AudioClip m_sosAudio;
        public AudioClip SosAudio => m_sosAudio;

        [SerializeField] private AudioClip m_cancelAudio;
        public AudioClip CancelAudio => m_cancelAudio;
    }
}