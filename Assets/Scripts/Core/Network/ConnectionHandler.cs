using System.Collections.Generic;
using Action = System.Action;

using Photon.Realtime;
using Photon.Pun;

namespace UnitWarfare.Network
{
    public sealed class ConnectionHandler : IConnectionCallbacks
    {
        public event Action ConnectedToMaster;
        public event Action FailedToConnect;

        internal ConnectionHandler()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                ConnectedToMaster?.Invoke();
                return;
            }
            PhotonNetwork.ConnectUsingSettings();
        }

        public void Disconnect() => PhotonNetwork.Disconnect();

        public void OnConnectedToMaster() =>
            ConnectedToMaster?.Invoke();

        public void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.Debug.Log(cause);
            // TODO: Disconnected causes
            FailedToConnect?.Invoke();
        }

        public void OnConnected() { }

        public void OnCustomAuthenticationFailed(string debugMessage) { }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

        public void OnRegionListReceived(RegionHandler regionHandler) { }
    }
}
