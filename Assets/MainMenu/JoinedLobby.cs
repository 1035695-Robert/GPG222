using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinedLobby : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerUI;
    [SerializeField] private Button joinButton;

    [SerializeField] private GameObject joinLobbyUI;
    [SerializeField] public GameObject localUi;


    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        localUi.SetActive(true);
        joinLobbyUI.SetActive(false);
        NetworkManager.Singleton.OnClientDisconnectCallback += LobbyDisconnectionManager.Instance.ClientDisconnectedHandler;
        NetworkManager.Singleton.OnTransportFailure += LobbyDisconnectionManager.Instance.NetworkTransportFailed;
       
        if (!IsServer) return;
        Debug.Log("joined");

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandler;
    }


    private void ClientConnectedHandler(ulong clientId)
    {
        NetworkObject networkPlayerUI = Instantiate(localPlayerUI).GetComponent<NetworkObject>();
        networkPlayerUI.SpawnAsPlayerObject(clientId, true);
        networkPlayerUI.TrySetParent(localUi.transform, false);
    }
    

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandler;
        NetworkManager.Singleton.OnTransportFailure -= LobbyDisconnectionManager.Instance.NetworkTransportFailed;

        NetworkManager.Singleton.OnClientDisconnectCallback -= LobbyDisconnectionManager.Instance.ClientDisconnectedHandler;
    }
}