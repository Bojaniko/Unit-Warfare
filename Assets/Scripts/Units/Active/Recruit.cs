using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class Recruit : SoldierUnit
    {
        public Recruit(Territory starting_territory, GameObject game_object, SoldierData data, IUnitTeamManager manager)
            : base(starting_territory, game_object, data, manager)
        {

        }
    }
}
