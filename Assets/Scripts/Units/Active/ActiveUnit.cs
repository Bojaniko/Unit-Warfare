using System.Collections;

using UnityEngine;

using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public abstract class ActiveUnit<D> : Unit<D>, IActiveUnit
        where D : ActiveUnitData
    {
        protected ActiveUnit(Territory start_territory, GameObject game_object, D data, IUnitOwner manager)
            : base(start_territory, game_object, data, manager)
        {
        }

        protected abstract override void OnDestroyed();

        protected override sealed void StartCommandRoutine(IUnitCommand command) =>
            StartCommand(command as UnitCommand<ActiveCommandOrder>);

        private void StartCommand(UnitCommand<ActiveCommandOrder> command)
        {
            if (command == null)
                return;

            if (command.Order.Equals(ActiveCommandOrder.CANCEL))
            {
                _emb.StartCoroutine(CancelCommandRoutine());
                return;
            }

            IEnumerator routine = null;

            switch (command.Order)
            {
                case ActiveCommandOrder.ATTACK:
                    routine = AttackCommandRoutine();
                    break;

                case ActiveCommandOrder.MOVE:
                    routine = MoveCommandRoutine();
                    break;

                case ActiveCommandOrder.JOIN:
                    routine = JoinCommandRoutine();
                    break;
            }
            if (routine == null)
                return;
            _emb.StartCoroutine(StartRoutine(routine, command));
        }

        private IEnumerator StartRoutine(IEnumerator routine, UnitCommand<ActiveCommandOrder> command)
        {
            OnCommandStart?.Invoke(this, command);
            yield return new WaitUntil(CommandIsReady);
            yield return routine;
            OnCommandEnd?.Invoke(this, command);
        }

        public override sealed event IUnit.Command OnCommandStart;
        public override sealed event IUnit.Command OnCommandEnd;

        public abstract event IActiveUnit.Command OnAttack;
        public abstract event IActiveUnit.Command OnMove;
        public abstract event IActiveUnit.Command OnJoin;

        protected abstract IEnumerator AttackCommandRoutine();
        protected abstract IEnumerator MoveCommandRoutine();
        protected abstract IEnumerator JoinCommandRoutine();
        protected abstract IEnumerator CancelCommandRoutine();

        protected abstract bool CommandIsReady();
        protected override abstract IEnumerator DamagedRoutine();
    }
}
