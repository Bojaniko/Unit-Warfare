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
        private bool _moveAvailable = false;
        public virtual bool MoveAvailable { get => _moveAvailable; protected set { _moveAvailable = value; } }

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
        }
        public event UnitsEventHandler OnDestroy;
        protected abstract void OnDestroyed();

        private IEnumerator KillUnit()
        {
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
        }

        // ##### COMMANDS ##### \\

        public abstract event IUnit.Command OnCommandStart;
        public abstract event IUnit.Command OnCommandEnd;
        public abstract void StartCommand(IUnitCommand command);
        public abstract IUnitCommand CurrentCommand { get; }
        public abstract bool IsCommandActive { get; }

        // ##### INITIALIZATION ##### \\

        private readonly D _data;
        UnitData IUnit.Data => Data;
        public D Data => _data;

        protected Unit(Territory start_territory, GameObject game_object, D data, IUnitTeamManager manager)
        {
            _data = data;
            _emb = new(this, game_object);

            _moveAvailable = false;

            _attack = data.Attack;
            _health = data.Health;
            _shield = data.Shield;

            manager.OnRoundStarted += () => _moveAvailable = true;

            _owner = start_territory.Owner.OwnerIdentification;

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