using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitWarfare.Core.Enums;
using UnitWarfare.Territories;

namespace UnitWarfare.Units
{
    public class Antennae : Unit<AntennaeData>, IPassiveUnit
    {
        public delegate void AntennaeEventHandler(Territory territory, SoldierData recruit_data);
        public event AntennaeEventHandler OnReinforce;

        private const string SIGNAL_LIGHT_NAME = "top_light";

        private readonly AntennaeSignalLight c_signalLight;
        private readonly AudioSource c_audioSource;

        private readonly IUnitTeamManager.UnitOwnerEventHandler f_enableSignal;
        private readonly IUnitTeamManager.UnitOwnerEventHandler f_disableSignal;

        public Antennae(Territory start_territory, GameObject game_object, AntennaeData data, IUnitTeamManager manager)
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

        private IUnitCommand _currentCommand;
        public override IUnitCommand CurrentCommand => _currentCommand;

        private bool _isCommandActive = false;
        public override bool IsCommandActive => _isCommandActive;

        public override event IUnit.Command OnCommandStart;
        public override event IUnit.Command OnCommandEnd;

        public override void StartCommand(IUnitCommand command) =>
            StartCommand(command as UnitCommand<AntennaeCommandOrder>);

        private void StartCommand(UnitCommand<AntennaeCommandOrder> command)
        {
            if (command == null)
                return;
            _currentCommand = command;
            if (command.Order.Equals(AntennaeCommandOrder.GENERATE_UNIT))
                _emb.StartCoroutine(CallRecruitReinforcment(command));
            else if (command.Order.Equals(AntennaeCommandOrder.CANCEL))
                _emb.StartCoroutine(CancelReincforcment());
        }

        private IEnumerator CancelReincforcment()
        {
            _currentCommand = null;

            c_audioSource.clip = Data.CancelAudio;
            c_audioSource.Play();

            yield return null;
        }

        private IEnumerator CallRecruitReinforcment(UnitCommand<AntennaeCommandOrder> command)
        {
            _isCommandActive = true;
            MoveAvailable = false;
            OnCommandStart?.Invoke(this, command);

            c_audioSource.clip = Data.SosAudio;
            c_audioSource.Play();

            OnReinforce?.Invoke(command.Target.Territory, Data.RecruitData);

            yield return new WaitForSeconds(1f);

            OnCommandEnd?.Invoke(this, command);
            _isCommandActive = false;
            _currentCommand = null;
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