using System.Collections;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Units
{
    public abstract class Unit<D> : IUnit
        where D : UnitData
    {
        // ##### TERRITORY ##### \\

        private readonly PlayerIdentification m_owner;
        public PlayerIdentification Owner => m_owner;

        private Territory m_occupiedTerritory;
        public Territory OccupiedTerritory
        {
            get => m_occupiedTerritory;
            protected set
            {
                m_occupiedTerritory = value;
            }
        }

        public void SetOccupiedTerritory(Territory territory)
        {
            if (territory.Equals(m_occupiedTerritory))
                return;
            m_occupiedTerritory = territory;
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
            m_occupiedTerritory.SetInteractable(false);
            m_isDoingSomething = true;
            yield return KillRoutine();
            DestroyUnit();
        }
        protected abstract IEnumerator KillRoutine();

        private readonly int m_shield;
        public int Shield => m_shield;

        private int m_health;
        public int Health => m_health;

        private int m_healthPercentage;
        public int HealthPercentage => m_healthPercentage;

        private readonly int m_attack;
        public int Attack => m_attack;

        public bool IsDead => (m_health <= 0 || _emb == null);

        public void Damage(int amount)
        {
            amount -= m_shield;
            if (amount < 0)
                amount = 0;
            m_health -= amount;
            if (m_health <= 0)
                m_health = 0;
            m_healthPercentage = m_health / _data.Health;

            if (m_health == 0)
                _emb.StartCoroutine(KillUnit());
            else
                _emb.StartCoroutine(DamagedRoutine());
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

            m_attack = data.Attack;
            m_health = data.Health;
            m_shield = data.Shield;

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

            m_owner = start_territory.Owner.Identification;

            m_occupiedTerritory = start_territory;
            m_occupiedTerritory.Occupy(this);
        }

        protected UnitEMB _emb;
        public UnitEMB EMB => _emb;
    }

    public class UnitEMB : EncapsulatedMonoBehaviour
    {
        private readonly IUnit m_unit;
        public IUnit Unit => m_unit;

        public UnitEMB(IUnit unit, GameObject game_object) : base(game_object)
        {
            m_unit = unit;
        }
    }
}