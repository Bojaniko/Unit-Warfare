using System.Collections.Generic;
using System.Collections;
using Action = System.Action;

using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

using UnitWarfare.Core;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UnitWarfare.Network
{
    public class MatchmackingHandler : IMatchmakingCallbacks, IInRoomCallbacks
    {
        public event Action OnFail;
        public event Action OnMatched;

        private readonly EncapsulatedMonoBehaviour emb;

        private Coroutine coroutine_matchFinder;

        internal MatchmackingHandler()
        {
            PhotonNetwork.AddCallbackTarget(this);
            emb = new(new("HANDLER:MATCHMACKING"));
            emb.transform.parent = NetworkHandler.Instance.transform;
        }

        private bool _failedJoinRoom = false;
        private bool _failedCreateRoom = false;

        public void FindMatch()
        {
            if (PhotonNetwork.CurrentRoom != null)
                return;

            _failedJoinRoom = false;
            _failedCreateRoom = false;

            coroutine_matchFinder = emb.StartCoroutine(FindMatchRoutine());
        }

        public void StopMatching()
        {
            PhotonNetwork.LeaveRoom();
            emb.StopCoroutine(coroutine_matchFinder);
            coroutine_matchFinder = null;
        }

        private IEnumerator FindMatchRoutine()
        {
            yield return FindRoomRoutine();

            if (PhotonNetwork.CurrentRoom == null)
                yield return CreateRoomRoutine();

            if (PhotonNetwork.CurrentRoom == null)
                OnFail?.Invoke();
        }
        private IEnumerator FindRoomRoutine()
        {
            PhotonNetwork.JoinRandomRoom();

            yield return new WaitUntil(() => (_failedJoinRoom || PhotonNetwork.CurrentRoom != null));
        }

        private IEnumerator CreateRoomRoutine()
        {
            RoomOptions options = new();
            options.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(null, options);

            yield return new WaitUntil(() => (_failedCreateRoom || PhotonNetwork.CurrentRoom != null));
        }

        public void OnJoinRandomFailed(short returnCode, string message) =>
            _failedJoinRoom = true;

        public void OnCreateRoomFailed(short returnCode, string message) =>
            _failedCreateRoom = true;

        public void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        OnMatched?.Invoke();
                        return;
                    }
                }
            }
        }

        public void OnPlayerEnteredRoom(Player newPlayer) =>
            OnMatched?.Invoke();

        // Unused callbacks.
        public void OnCreatedRoom() { }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnLeftRoom() { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
        public void OnPlayerLeftRoom(Player otherPlayer) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
    }
}
