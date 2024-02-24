using UnityEngine;

using PhotonNetwork = Photon.Pun.PhotonNetwork;

namespace UnitWarfare.Network
{
    public class NetworkHandler : MonoBehaviour
    {
        private static NetworkHandler m_instance;
        public static NetworkHandler Instance => m_instance;

        public static void CreateInstance()
        {
            if (m_instance != null)
                return;
            GameObject go = new("NETWORK_HANDLER");
            NetworkHandler nh = go.AddComponent<NetworkHandler>();
            DontDestroyOnLoad(nh);
            m_instance = nh;
            m_instance.m_connection = new();
            m_instance.m_matchmacking = new();
            m_instance.m_roomHandler = new();
        }

        private void Awake()
        {
            if (m_instance == null)
                throw new UnityException("Please use NetworkHandler.CreateInstance() for instancing the network handler.");
        }

        public bool IsHost => PhotonNetwork.IsMasterClient;

        private ConnectionHandler m_connection;
        public ConnectionHandler Connection => m_connection;

        private MatchmackingHandler m_matchmacking;
        public MatchmackingHandler Matchmacking => m_matchmacking;

        private RoomHandler m_roomHandler;
        public RoomHandler RoomHandler => m_roomHandler;
    }
}