using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitWarfare.Core.Global;
using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class Antennae : Unit<AntennaeData>, IPassiveUnit
    {
        public delegate void AntennaeEventHandler(Territory territory);
        public event AntennaeEventHandler OnReinforce;

        private const string SIGNAL_LIGHT_NAME = "top_light";

        private readonly AntennaeSignalLight c_signalLight;
        private readonly AudioSource c_audioSource;

        private readonly IUnitOwner.UnitOwnerEventHandler f_enableSignal;
        private readonly IUnitOwner.UnitOwnerEventHandler f_disableSignal;

        public Antennae(Territory start_territory, GameObject game_object, AntennaeData data, IUnitOwner manager)
            : base(start_territory, game_object, data, manager)
        {
            foreach (Transform t in _emb.transform)
            {
                if (t.name.Equals(SIGNAL_LIGHT_NAME))
                {
                    c_signalLight = t.GetComponent<AntennaeSignalLight>();
                }
            }

            c_audioSource = _emb.gameObject.GetComponent<AudioSource>();

            f_enableSignal = c_signalLight.EnableSignal;
            f_disableSignal = c_signalLight.DisableSignal;

            manager.OnRoundStarted += f_enableSignal;
            manager.OnRoundEnded += f_disableSignal;
        }

        public override event IUnit.Command OnCommandStart;
        public override event IUnit.Command OnCommandEnd;

        protected override void StartCommandRoutine(IUnitCommand command) =>
            StartCommand(command as UnitCommand<AntennaeCommandOrder>);

        private void StartCommand(UnitCommand<AntennaeCommandOrder> command)
        {
            if (command == null)
                return;
            if (command.Order.Equals(AntennaeCommandOrder.GENERATE_UNIT))
                _emb.StartCoroutine(CallRecruitReinforcment(command));
            else if (command.Order.Equals(AntennaeCommandOrder.CANCEL) || command.Order.Equals(AntennaeCommandOrder.SKIP))
                _emb.StartCoroutine(CancelReincforcment());
        }

        private IEnumerator CancelReincforcment()
        {
            c_audioSource.clip = Data.CancelAudio;
            c_audioSource.Play();

            yield return null;
        }

        private IEnumerator CallRecruitReinforcment(UnitCommand<AntennaeCommandOrder> command)
        {
            OnCommandStart?.Invoke(this, command);

            c_audioSource.clip = Data.SosAudio;
            c_audioSource.Play();

            OnReinforce?.Invoke(command.Target.Territory);

            yield return new WaitForSeconds(1f);

            OnCommandEnd?.Invoke(this, command);
        }

        protected override IEnumerator KillRoutine()
        {
            // TODO: Simulate explosion
            Debug.Log("EXPLODE!");
            yield return null;
        }

        protected override void OnDestroyed()
        {
            manager.OnRoundStarted -= f_enableSignal;
            manager.OnRoundEnded -= f_disableSignal;
        }

        protected override IEnumerator DamagedRoutine()
        {
            return null;
        }
    }
}