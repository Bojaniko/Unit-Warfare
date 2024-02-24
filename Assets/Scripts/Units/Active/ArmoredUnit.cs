using System.Collections;
using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class ArmoredUnit : ActiveUnit<ArmoredUnitData>
    {
        private readonly Mover mover;

        private readonly Animator animator;

        private const string DRIVE_ANIMATION_NAME = "DRIVE";
        private const string EXIT_ANIMATION_NAME = "IDLE";

        public ArmoredUnit(Territory territory, GameObject game_object, ArmoredUnitData data, IUnitOwner manager)
            : base(territory, game_object, data, manager)
        {
            animator = game_object.GetComponent<Animator>();
            mover = new(_emb, data.Speed);
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
            _emb.transform.LookAt(CurrentCommand.Target.Territory.EMB.transform);
            mover.MoveToDestination(CurrentCommand.Target.Territory.EMB.transform.position);
            animator.Play(DRIVE_ANIMATION_NAME);

            yield return new WaitUntil(() => mover.CurrentState.Equals(MoverState.WAITING));

            animator.Play(EXIT_ANIMATION_NAME);
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
            Debug.Log("Armored Unit damaged");
            yield return null;
        }
    }
}