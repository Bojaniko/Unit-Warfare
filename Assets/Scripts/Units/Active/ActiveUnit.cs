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

        private bool _commandActive = false;
        public override sealed bool IsCommandActive => _commandActive;
        private UnitCommand<ActiveCommandOrder> _currentCommand;
        public override sealed IUnitCommand CurrentCommand => _currentCommand;

        protected abstract override void OnDestroyed();

        public override sealed void StartCommand(IUnitCommand command)
        {
            if (_currentCommand != null)
                return;
            StartCommand(command as UnitCommand<ActiveCommandOrder>);
        }

        private void StartCommand(UnitCommand<ActiveCommandOrder> command)
        {
            if (command == null)
                return;

            if (!MoveAvailable)
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
            _commandActive = true;
            _currentCommand = command;
            _emb.StartCoroutine(CommandRoutine(routine));
        }

        private IEnumerator CommandRoutine(ActiveCommandRoutine command_routine)
        {
            OnCommandStart?.Invoke(this, CurrentCommand);
            yield return command_routine.RoutineDelegate.Invoke();
            OnCommandEnd?.Invoke(this, CurrentCommand);
            _commandActive = false;
            _currentCommand = null;
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
    }
}
