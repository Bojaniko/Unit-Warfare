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

        public override event IActiveUnit.Command OnAttack;
        public override event IActiveUnit.Command OnMove;
        public override event IActiveUnit.Command OnJoin;

        protected override IEnumerator AttackCommandRoutine()
        {
            yield return null;
            OnAttack?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator JoinCommandRoutine()
        {
            yield return null;
            OnJoin?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator MoveCommandRoutine()
        {
            yield return null;
            _emb.transform.position = CurrentCommand.Target.Territory.EMB.transform.position;
            OnMove?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator CancelCommandRoutine()
        {
            yield return null;
        }

        protected override IEnumerator KillRoutine()
        {
            yield return null;
        }

        protected override void OnDestroyed()
        {
            
        }

        protected override bool CommandIsReady() => true;

        protected override IEnumerator DamagedRoutine()
        {
            yield return null;
        }
    }
}