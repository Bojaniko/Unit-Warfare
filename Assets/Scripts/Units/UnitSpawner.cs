using System;
using System.Collections;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Territories;
using UnitWarfare.Core.Global;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace UnitWarfare.Units
{
    public class UnitNetworkSerialization
    {
        public byte TerritoryId { get; set; }
        public string UnitType { get; set; }

        public UnitNetworkSerialization(byte territory_id, string unit_type)
        {
            TerritoryId = territory_id;
            UnitType = unit_type;
        }
    }

    public sealed class NetworkUnitSpawner : UnitSpawner, IOnEventCallback, IDisposable
    {
        private bool m_canSpawn;
        public override bool CanSpawn => m_canSpawn;

        public NetworkUnitSpawner(IUnitsHandler handler)
            : base(handler)
        {
            PhotonNetwork.AddCallbackTarget(this);
            m_canSpawn = true;
        }

        public void Dispose() =>
            PhotonNetwork.RemoveCallbackTarget(this);

        public override event Action<IUnit> OnSpawn;
        public override event Action<IUnit> OnDespawn;

        protected override void SubDespawn(IUnit unit)
        {
            OnDespawn?.Invoke(unit);
            m_canSpawn = false;

            coroutine_confirmationCoroutine = emb.StartCoroutine(SpawnConfirmationRoutine());

            uns_lastSent = new(unit.OccupiedTerritory.ID, unit.GetType().Name);
            PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_DESPAWN_UNIT_CODE, uns_lastSent, null, SendOptions.SendReliable);

            unit.DestroyUnit();
        }

        protected override void SubSpawn(Territory territory, Type type)
        {
            IUnitOwner owner = (IUnitOwner)territory.Owner;
            if (owner == null)
                throw new UnityException("Territory owners must always be IUnitOwner and Player types.");
            UnitData data = handler.GetUnitDataByUnit(owner, type);

            IUnit unit = UnitFactory.GenerateUnit(territory, data, type, handler.UnitContainer, owner);
            OnSpawn?.Invoke(unit);
            m_canSpawn = false;

            coroutine_confirmationCoroutine = emb.StartCoroutine(SpawnConfirmationRoutine());

            uns_lastSent = new(territory.ID, type.Name);
            PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_SPAWN_UNIT_CODE, uns_lastSent, null, SendOptions.SendReliable);
        }

        private UnitNetworkSerialization uns_lastSent;
        private UnitNetworkSerialization uns_lastReceived;

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code.Equals(GlobalValues.NETWORK_SPAWN_UNIT_CODE))
            {
                uns_lastReceived = (UnitNetworkSerialization)photonEvent.CustomData;
                if (!uns_lastReceived.TerritoryId.Equals(uns_lastSent.TerritoryId))
                {
                    if (coroutine_confirmationCoroutine != null)
                        emb.StopCoroutine(coroutine_confirmationCoroutine);
                    coroutine_confirmationCoroutine = emb.StartCoroutine(SpawnConfirmationRoutine());
                }
                else
                    PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_SPAWN_UNIT_CONFIRMED_CODE, uns_lastReceived, null, SendOptions.SendReliable);
            }
            else if (photonEvent.Code.Equals(GlobalValues.NETWORK_DESPAWN_UNIT_CODE))
            {
                uns_lastReceived = (UnitNetworkSerialization)photonEvent.CustomData;
                if (!uns_lastReceived.TerritoryId.Equals(uns_lastSent.TerritoryId))
                {
                    if (coroutine_confirmationCoroutine != null)
                        emb.StopCoroutine(coroutine_confirmationCoroutine);
                    coroutine_confirmationCoroutine = emb.StartCoroutine(SpawnConfirmationRoutine());
                }
                else
                    PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_DESPAWN_UNIT_CONFIRMED_CODE, uns_lastReceived, null, SendOptions.SendReliable);
            }
            else if (photonEvent.Code.Equals(GlobalValues.NETWORK_SPAWN_UNIT_CONFIRMED_CODE)
                || photonEvent.Code.Equals(GlobalValues.NETWORK_DESPAWN_UNIT_CONFIRMED_CODE))
            {
                uns_lastReceived = (UnitNetworkSerialization)photonEvent.CustomData;
                if (uns_lastReceived.TerritoryId == uns_lastSent.TerritoryId)
                {
                    if (coroutine_confirmationCoroutine != null)
                    {
                        emb.StopCoroutine(coroutine_confirmationCoroutine);
                        coroutine_confirmationCoroutine = null;
                    }
                    m_canSpawn = true;
                }
            }
        }

        private Coroutine coroutine_confirmationCoroutine;

        private IEnumerator SpawnConfirmationRoutine()
        {
            yield return new WaitForSeconds(GlobalValues.NETWORK_MAX_RESPONSE_DELAY_SECONDS);
            coroutine_confirmationCoroutine = null;
            //TODO: Close match due to no confirmation
        }
    }

    public sealed class LocalUnitSpawner : UnitSpawner
    {
        public override bool CanSpawn => true;

        public LocalUnitSpawner(IUnitsHandler handler)
            : base(handler)
        {

        }

        public override event Action<IUnit> OnSpawn;
        public override event Action<IUnit> OnDespawn;

        protected override void SubDespawn(IUnit unit)
        {
            OnDespawn?.Invoke(unit);
            unit.DestroyUnit();
        }

        protected override void SubSpawn(Territory territory, Type type)
        {
            IUnitOwner owner = (IUnitOwner)territory.Owner;
            if (owner == null)
                throw new UnityException("Territory owners must always be IUnitOwner and Player types.");
            UnitData data = handler.GetUnitDataByUnit(owner, type);

            IUnit unit = UnitFactory.GenerateUnit(territory, data, type, handler.UnitContainer, owner);
            OnSpawn?.Invoke(unit);
        }
    }

    public abstract class UnitSpawner
    {
        public abstract bool CanSpawn { get; }

        protected readonly IUnitsHandler handler;

        protected readonly EncapsulatedMonoBehaviour emb;

        protected UnitSpawner(IUnitsHandler handler)
        {
            this.handler = handler;
            emb = new(new("UNITS_HANDLER:SPAWNER"));
        }

        public abstract event Action<IUnit> OnSpawn;
        public abstract event Action<IUnit> OnDespawn;

        public void Spawn(Territory territory, Type type)
        {
            if (!CanSpawn)
                return;
            SubSpawn(territory, type);
        }

        public void Despawn(IUnit unit)
        {
            if (!CanSpawn)
                return;
            SubDespawn(unit);
        }

        protected abstract void SubSpawn(Territory territory, Type type);
        protected abstract void SubDespawn(IUnit unit);
    }
}
