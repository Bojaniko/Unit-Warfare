using System.Collections;
using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class ArmoredUnit : ActiveUnit<ArmoredUnitData>
    {
        public ArmoredUnit(Territory territory, GameObject game_object, ArmoredUnitData data, IUnitTeamManager manager)
            : base(territory, game_object, data, manager)
        {

        }

        public override event UnitAttack OnAttack;
        public override event UnitMove OnMove;
        public override event UnitJoin OnJoin;

        protected override IEnumerator AttackCommandRoutine()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator JoinCommandRoutine()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator MoveCommandRoutine()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator CancelCommandRoutine()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator KillRoutine()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDestroyed()
        {
            throw new System.NotImplementedException();
        }
    }
}