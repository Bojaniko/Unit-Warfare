using System.Collections.Generic;
using Action = System.Action;
using IDisposable = System.IDisposable;

using Photon.Realtime;
using Photon.Pun;

namespace UnitWarfare.Network
{
    public sealed class ConnectionHandler : IConnectionCallbacks, IDisposable
    {
        public event Action ConnectedToMaster;
        public event Action FailedToConnect;

        public ConnectionHandler()
        {
            PhotonNetwork.AddCallbackTarget(this);
            PhotonNetwork.ConnectUsingSettings();
        }

        public void OnConnected() { }

        public void OnConnectedToMaster()
        {
            ConnectedToMaster?.Invoke();
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            // TODO: Disconnected causes
            FailedToConnect?.Invoke();
        }

        public void OnCustomAuthenticationFailed(string debugMessage) { }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

        public void OnRegionListReceived(RegionHandler regionHandler) { }

        public void Dispose()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
    }
}
