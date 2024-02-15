using UnityEngine;

using UnitWarfare.Core.Global;

namespace UnitWarfare.Players
{
    public class PlayerData
    {
        private readonly string m_name;
        public string Name => m_name;

        private readonly Nation m_nation;
        public Nation Nation => m_nation;

        public PlayerData(string name, Nation nation)
        {
            m_name = name;
            m_nation = nation;
        }
    }
}
