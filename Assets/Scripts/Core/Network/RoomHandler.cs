using Action = System.Action;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

using UnitWarfare.Core.Global;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

namespace UnitWarfare.Network
{
    public class RoomHandler : IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
    {
        public event System.Action<byte, Player> OnGameStarted;
        public event Action OnGameLeft;
        public event Action OtherPlayerDisconnected;

        private Player player_other;

        public RoomHandler()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private bool m_gameStarted;
        public bool IsGameStarted => m_gameStarted;

        public void StartGame(byte level)
        {
            if (PhotonNetwork.CurrentRoom == null)
                return;
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
                return;
            if (m_gameStarted)
                return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            RaiseEventOptions options = new();
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(GlobalValues.NETWORK_GAME_STARTED_CODE, level, options, SendOptions.SendReliable);
        }

        public void LeaveGame()
        {
            if (!m_gameStarted)
                return;
            PhotonNetwork.LeaveRoom();
            OnGameLeft?.Invoke();
        }

        // Handle the network event for when the game is started.
        public void OnEvent(EventData photonEvent)
        {
            if (!photonEvent.Code.Equals(GlobalValues.NETWORK_GAME_STARTED_CODE))
                return;
            if (!photonEvent.Sender.Equals(PhotonNetwork.CurrentRoom.MasterClientId))
                return;
            if (!photonEvent.CustomData.GetType().Equals(typeof(byte)))
                return;
            if (player_other == null)
                return;
            if (m_gameStarted)
                return;
            m_gameStarted = true;
            OnGameStarted?.Invoke((byte)photonEvent.CustomData, player_other);
        }

        // When the other player leaves the room, send a local event
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!m_gameStarted)
                return;
            if (!player_other.Equals(otherPlayer))
                return;
            player_other = null;
            PhotonNetwork.LeaveRoom();
            OtherPlayerDisconnected?.Invoke();
        }

        // When a player enters the room, set is as the other player
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
                return;
            if (player_other != null)
                return;
            player_other = newPlayer;
        }

        // Assign the other player when the local client enters a room.
        public void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 2) {
                PhotonNetwork.LeaveRoom();
                return;
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                return;
            foreach (Player other in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (other.Equals(PhotonNetwork.LocalPlayer))
                    continue;
                player_other = other;
                break;
            }
        }

        // Unused callbacks.
        public void OnMasterClientSwitched(Player newMasterClient) { }
        public void OnCreatedRoom() => player_other = null;
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnJoinRandomFailed(short returnCode, string message) { }
        public void OnLeftRoom() { }
    }
}
