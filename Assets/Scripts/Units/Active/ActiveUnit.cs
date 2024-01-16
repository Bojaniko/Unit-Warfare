using System.Collections;

using UnityEngine;

using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public abstract class ActiveUnit<D> : Unit<D>, IActiveUnit
        where D : ActiveUnitData
    {
        public record ActiveCommandRoutine(System.Func<IEnumerator> RoutineDelegate, ActiveCommandOrder CommandOrder);

        protected ActiveUnit(Territory start_territory, GameObject game_object, D data, IUnitTeamManager manager)
            : base(start_territory, game_object, data, manager)
        {
            InitRoutines();
        }

        private ActiveCommandRoutine[] _routines;

        private void InitRoutines()
        {
            _routines = new ActiveCommandRoutine[4];
            _routines[0] = new(AttackCommandRoutine, ActiveCommandOrder.ATTACK);
            _routines[1] = new(MoveCommandRoutine, ActiveCommandOrder.MOVE);
            _routines[2] = new(JoinCommandRoutine, ActiveCommandOrder.JOIN);
            _routines[3] = new(CancelCommandRoutine, ActiveCommandOrder.CANCEL);
        }

        private UnitCommand<ActiveCommandOrder> _currentCommand;
        public override IUnitCommand CurrentCommand => _currentCommand;
        UnitCommand<ActiveCommandOrder> IActiveUnit.CurrentCommand => _currentCommand;

        protected abstract override void OnDestroyed();

        public override void StartCommand(IUnitCommand command)
        {
            if (command is UnitCommand<ActiveCommandOrder>)
                StartCommand(command as UnitCommand<ActiveCommandOrder>);
        }

        private void StartCommand(UnitCommand<ActiveCommandOrder> command)
        {
            if (_currentCommand != null)
                return;

            ActiveCommandRoutine routine = null;
            foreach (ActiveCommandRoutine r in _routines)
            {
                if (r.CommandOrder.Equals(command.Order))
                {
                    routine = r;
                    break;
                }
            }

            if (routine == null)
                return;

            MoveAvailable = false;
            _currentCommand = command;
            _emb.StartCoroutine(CommandRoutine(routine));
        }

        private IEnumerator CommandRoutine(ActiveCommandRoutine command_routine)
        {
            yield return command_routine.RoutineDelegate.Invoke();
            _currentCommand = null;
        }

        public abstract event UnitAttack OnAttack;
        public abstract event UnitMove OnMove;
        public abstract event UnitJoin OnJoin;

        protected abstract IEnumerator AttackCommandRoutine();
        protected abstract IEnumerator MoveCommandRoutine();
        protected abstract IEnumerator JoinCommandRoutine();
        protected abstract IEnumerator CancelCommandRoutine();

        void IActiveUnit.StartCommand(UnitCommand<ActiveCommandOrder> command)
        {
            throw new System.NotImplementedException();
        }
    }
}
