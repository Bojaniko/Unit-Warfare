using System.Collections;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Territories;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.Units
{
    public abstract class Unit<D> : IUnit
        where D : UnitData
    {
        // ##### TERRITORY ##### \\

        private readonly PlayerIdentification _owner;
        public PlayerIdentification Owner => _owner;

        private Territory _occupiedTerritory;
        public Territory OccupiedTerritory
        {
            get => _occupiedTerritory;
            protected set
            {
                _occupiedTerritory = value;
            }
        }

        public void SetOccupiedTerritory(Territory territory)
        {
            if (territory.Equals(_occupiedTerritory))
                return;
            _occupiedTerritory = territory;
        }

        // ##### DESTRUCTION ##### \\

        public void DestroyUnit()
        {
            OnDestroyed();

            OnDestroy?.Invoke(this);

            _emb.Destroy();
            _emb = null;
        }
        public event UnitsEventHandler OnDestroy;
        protected abstract void OnDestroyed();

        private IEnumerator KillUnit()
        {
            _occupiedTerritory.SetInteractable(false);
            m_isDoingSomething = true;
            yield return KillRoutine();
            DestroyUnit();
        }
        protected abstract IEnumerator KillRoutine();

        private readonly int _shield;
        public int Shield => _shield;

        private int _health;
        public int Health => _health;

        private readonly int _attack;
        public int Attack => _attack;

        public bool IsDead => (_health <= 0 || _emb == null);

        public void Damage(int amount)
        {
            amount -= _shield;
            if (amount < 0)
                amount = 0;
            _health -= amount;
            if (_health <= 0)
            {
                _health = 0;
                _emb.StartCoroutine(KillUnit());
            }
            else _emb.StartCoroutine(DamagedRoutine());
        }

        protected abstract IEnumerator DamagedRoutine();

        // ##### COMMANDS ##### \\

        public abstract event IUnit.Command OnCommandStart;
        public abstract event IUnit.Command OnCommandEnd;
        public void StartCommand(IUnitCommand command)
        {
            if (command == null)
                return;
            if (!MoveAvailable)
                return;
            if (IsDoingSomething)
                return;
            StartCommandRoutine(command);
        }

        protected abstract void StartCommandRoutine(IUnitCommand command);

        private IUnitCommand m_currentCommand;
        public IUnitCommand CurrentCommand => m_currentCommand;

        private bool m_isCommandActive = false;
        private bool m_isDoingSomething = false;
        public bool IsDoingSomething => (m_isCommandActive || m_isDoingSomething);

        private bool m_moveAvailable = false;
        public virtual bool MoveAvailable => m_moveAvailable;

        public int CompareTo(IUnit other)
        {
            // TODO
            return 0;
        }

        // ##### INITIALIZATION ##### \\

        private readonly D _data;
        UnitData IUnit.Data => Data;
        public D Data => _data;

        protected readonly IUnitTeamManager manager;

        protected Unit(Territory start_territory, GameObject game_object, D data, IUnitTeamManager manager)
        {
            _data = data;
            _emb = new(this, game_object);

            m_moveAvailable = false;

            _attack = data.Attack;
            _health = data.Health;
            _shield = data.Shield;

            this.manager = manager;
            manager.OnRoundStarted += () => { m_moveAvailable = true; };

            OnCommandStart += (unit, command) =>
            {
                m_isCommandActive = true;
                m_currentCommand = command;
                m_moveAvailable = false;
            };

            OnCommandEnd += (unit, command) =>
            {
                m_isCommandActive = false;
                m_currentCommand = null;
            };

            _owner = start_territory.Owner.Identification;

            _occupiedTerritory = start_territory;
            _occupiedTerritory.Occupy(this);
        }

        // ##### MONO BEHAVIOUR ##### \\

        protected UnitEMB _emb;
        public UnitEMB EMB => _emb;

        public int HealthPercentage => throw new System.NotImplementedException();
    }

    public class UnitEMB : EncapsulatedMonoBehaviour
    {
        private readonly IUnit _unit;
        public IUnit Unit => _unit;

        public UnitEMB(IUnit unit, GameObject game_object) : base(game_object)
        {
            _unit = unit;
        }
    }
}