using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinedLobby : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerUI;
    [SerializeField] private Button joinButton;

    [SerializeField] private GameObject joinLobbyUI;
    [SerializeField] public GameObject localUi;

    private NetworkList<PlayerColourSelectState> _players;

    private void Awake()
    {
        _players = new NetworkList<PlayerColourSelectState>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        localUi.SetActive(true);
        joinLobbyUI.SetActive(false);
        
        if (!IsServer) return;
        Debug.Log("joined");
        
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandler;
        NetworkManager.Singleton.OnClientDisconnectCallback += CLientDisconnectedHandler;
    }
    
    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandler;
        NetworkManager.Singleton.OnClientDisconnectCallback -= CLientDisconnectedHandler;
    }

    private void ClientConnectedHandler(ulong clientId)
    {
        NetworkObject networkPlayerUI = Instantiate(localPlayerUI).GetComponent<NetworkObject>();
        networkPlayerUI.SpawnAsPlayerObject(clientId, true);
        networkPlayerUI.TrySetParent(localUi.transform, false);
    }

    private void CLientDisconnectedHandler(ulong clientId)
    {
       
    }

   

}