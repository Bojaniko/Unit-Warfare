using System.Collections;
using Action = System.Action;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

using UnitWarfare.Core;
using UnitWarfare.Core.Global;

namespace UnitWarfare.Network
{
    public class GameEventsHandler
    {
        private readonly EncapsulatedMonoBehaviour emb;
        internal GameEventsHandler()
        {
            emb = new(new("HANDLER:GAME_EVENTS"));
            emb.transform.parent = NetworkHandler.Instance.transform;
        }

        private IEnumerator TryResend(EventSendData data)
        {
            bool success = false;
            for (byte i = 0; i < GlobalValues.NETWORK_REPEATED_EVENT_MAX_TRIES; i++)
            {
                yield return new WaitForSeconds(GlobalValues.NETWORK_REPEATED_EVENT_TRY_DELAY_MS / 1000);
                if (ResendEvent(data))
                {
                    success = true;
                    break;
                }
            }
            if (success)
                OnRepeatedNetworkEventSent?.Invoke();
            else
                OnNetworkEventFailed?.Invoke();
        }

        public event Action OnNetworkEventNotSent;
        public event Action OnRepeatedNetworkEventSent;
        public event Action OnNetworkEventFailed;

        private bool ResendEvent(EventSendData data) => PhotonNetwork.RaiseEvent(data.Code, data.Content, data.Options, data.SendOptions);

        public void RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            bool send = PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
            if (!send)
            {
                OnNetworkEventNotSent?.Invoke();
                emb.StartCoroutine(TryResend(new EventSendData(eventCode, eventContent, raiseEventOptions, sendOptions)));
            }
        }

        private sealed record EventSendData(byte Code, object Content, RaiseEventOptions Options, SendOptions SendOptions);
    }
}