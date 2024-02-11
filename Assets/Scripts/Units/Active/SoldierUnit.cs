using System.Collections;

using UnityEngine;

using UnitWarfare.Territories;

namespace UnitWarfare.Units
{   
    public class SoldierUnit : ActiveUnit<SoldierData>
    {
        private readonly Mover _mover;

        private readonly Animator _animator;

        private const string WALK_SPEED_MULT = "walk_speed";

        protected const string ANIMATION_NAME_DIE = "DIE";
        protected const string ANIMATION_NAME_WALK = "WALK";
        protected const string ANIMATION_NAME_IDLE = "IDLE";
        protected const string ANIMATION_NAME_SHOOT = "SHOOT";
        protected const string ANIMATION_NAME_SALUTE = "SALUTE";
        protected const string ANIMATION_NAME_WOUNDED = "SHOT";

        protected const string ANIMATOR_WALKING = "walking";
        protected const string ANIMATOR_SHOOTING = "shooting";
        protected const string ANIMATOR_SHOOT_MODE = "shoot_mode";

        protected const int ANIMATION_VARIATIONS_SHOOT = 2;

        public SoldierUnit(Territory starting_territory, GameObject game_object, SoldierData data, IUnitTeamManager manager)
            : base(starting_territory, game_object, data, manager)
        {
            _animator = game_object.GetComponent<Animator>();
            _mover = new(_emb, data.Speed);
            _mover.SetSpeedMultiplier(GetWalkSpeedMultiplier);
        }

        private float GetWalkSpeedMultiplier() =>
            _animator.GetFloat(WALK_SPEED_MULT);

        protected override IEnumerator AttackCommandRoutine()
        {
            _emb.transform.LookAt(CurrentCommand.Target.Territory.EMB.transform);

            int rand = Random.Range(0, ANIMATION_VARIATIONS_SHOOT);

            _animator.SetInteger(ANIMATOR_SHOOT_MODE, rand);

            _animator.SetBool(ANIMATOR_SHOOTING, true);
            _animator.Play(ANIMATION_NAME_SHOOT, 0);

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            yield return new WaitForSeconds(0.2f);

            OnAttack?.Invoke(this, CurrentCommand.Target);
            _animator.SetBool(ANIMATOR_SHOOTING, false);

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_NAME_IDLE));
        }

        protected override IEnumerator MoveCommandRoutine()
        {
            _emb.transform.LookAt(CurrentCommand.Target.Territory.EMB.transform);

            _animator.SetBool(ANIMATOR_WALKING, true);
            _animator.Play(ANIMATION_NAME_WALK, 0);

            _mover.MoveToDestination(CurrentCommand.Target.Territory.EMB.transform.position);

            yield return new WaitUntil(() => _mover.CurrentState.Equals(MoverState.WAITING));

            _animator.SetBool(ANIMATOR_WALKING, false);
            _animator.CrossFade(ANIMATION_NAME_IDLE, 0.1f, 0);

            OnMove?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator JoinCommandRoutine()
        {
            _emb.transform.LookAt(CurrentCommand.Target.Territory.EMB.transform);

            _animator.Play(ANIMATION_NAME_SALUTE, 0);

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            OnJoin?.Invoke(this, CurrentCommand.Target);
        }

        protected override IEnumerator CancelCommandRoutine()
        {
            Debug.Log("Cancelling soldier command");
            yield return null;
        }

        protected override void OnDestroyed()
        {
            Debug.Log("Destroying");
        }

        protected override IEnumerator KillRoutine()
        {
            _animator.Play(ANIMATION_NAME_DIE, 0);

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }

        protected override IEnumerator DamagedRoutine()
        {
            _animator.Play(ANIMATION_NAME_WOUNDED, 0);

            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }

        protected override bool CommandIsReady() =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_NAME_IDLE);

        public override event IActiveUnit.Command OnAttack;
        public override event IActiveUnit.Command OnMove;
        public override event IActiveUnit.Command OnJoin;
    }
}