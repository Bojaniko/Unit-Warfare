using System.Collections;

using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{   
    public class SoldierUnit : ActiveUnit<SoldierData>
    {
        private readonly Mover _mover;

        private readonly Animator _animator;

        private const string _walkSpeedMult = "walk_speed";

        public SoldierUnit(Territory starting_territory, GameObject game_object, SoldierData data, IUnitTeamManager manager)
            : base(starting_territory, game_object, data, manager)
        {
            _animator = game_object.GetComponent<Animator>();
            _mover = new(_emb, data.Speed);
            _mover.SetSpeedMultiplier(GetWalkSpeedMultiplier);
        }

        private float GetWalkSpeedMultiplier() =>
            _animator.GetFloat(_walkSpeedMult);

        protected override IEnumerator AttackCommandRoutine()
        {
            OnAttack?.Invoke(this, CurrentCommand.Target);

            return null;
        }

        protected override IEnumerator MoveCommandRoutine()
        {
            _emb.transform.LookAt(CurrentCommand.Target.Territory.EMB.transform);

            _animator.SetBool("walking", true);

            _mover.MoveToDestination(CurrentCommand.Target.Territory.EMB.transform.position);

            yield return new WaitUntil(() => _mover.CurrentState.Equals(MoverState.WAITING));

            _animator.SetBool("walking", false);
            _animator.Play("IDLE");

            OnMove?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator JoinCommandRoutine()
        {
            OnJoin?.Invoke(this, CurrentCommand.Target);

            return null;
        }

        protected override IEnumerator CancelCommandRoutine()
        {
            return null;
        }

        protected override void OnDestroyed()
        {
            Debug.Log("Destroying");
        }

        protected override IEnumerator KillRoutine()
        {
            Debug.Log("Killed");

            yield return new WaitForSeconds(0.2f);
        }

        public override event IActiveUnit.Command OnAttack;
        public override event IActiveUnit.Command OnMove;
        public override event IActiveUnit.Command OnJoin;
    }
}