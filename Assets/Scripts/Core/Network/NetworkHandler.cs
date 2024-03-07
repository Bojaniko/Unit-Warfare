using UnityEngine;

using PhotonNetwork = Photon.Pun.PhotonNetwork;

using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace UnitWarfare.Network
{
    public class NetworkHandler : MonoBehaviour
    {
        private static GameObject m_instanceGameObject;
        private static NetworkHandler m_instance;
        public static NetworkHandler Instance => m_instance;

        public static void CreateInstance()
        {
            if (m_instance != null)
                return;
            GameObject go = new("NETWORK:HANDLER");
            m_instanceGameObject = go;
            NetworkHandler nh = go.AddComponent<NetworkHandler>();
            DontDestroyOnLoad(nh);
            m_instance = nh;
            m_instance.m_connection = new();
            m_instance.m_matchmacking = new();
            m_instance.m_roomHandler = new();
            m_instance.m_gameEvents = new();
        }

        private void Awake()
        {
            if (!m_instanceGameObject.Equals(gameObject))
                throw new UnityException("Please use NetworkHandler.CreateInstance() for instancing the network handler.");
        }

        public bool IsHost => PhotonNetwork.IsMasterClient;

        private ConnectionHandler m_connection;
        public ConnectionHandler Connection => m_connection;

        private MatchmackingHandler m_matchmacking;
        public MatchmackingHandler Matchmacking => m_matchmacking;

        private RoomHandler m_roomHandler;
        public RoomHandler RoomHandler => m_roomHandler;

        private GameEventsHandler m_gameEvents;
        public GameEventsHandler GameEvents => m_gameEvents;
    }
}