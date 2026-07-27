using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinedLobby : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerUI;
    [SerializeField] private Button joinButton;

    [SerializeField] private GameObject joinLobbyUI;
    [SerializeField] public GameObject localUi;

    public override void OnNetworkSpawn()
    {
        Debug.Log("joined");
        localUi.SetActive(true);
        joinLobbyUI.SetActive(false);
        RequestJoin_Rpc(NetworkManager.Singleton.LocalClientId);
        
    }
    
    [Rpc(SendTo.Server)]
    private void RequestJoin_Rpc(ulong clientId)
    {
        NetworkObject networkPlayerUI = Instantiate(localPlayerUI).GetComponent<NetworkObject>();
        networkPlayerUI.SpawnWithOwnership(clientId, true);
        networkPlayerUI.TrySetParent(localUi.transform, false);
        
    }
}