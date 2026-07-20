using Player.hands;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;

   

[Header("PlayerManager")]
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject canvasUI;
    public int maxPlayerCount = 2;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }

    void SpawnPlayer(ulong clientID)
    {
        NetworkObject newPlayer = Instantiate(playerPrefab).GetComponent<NetworkObject>();
        newPlayer.SpawnAsPlayerObject(clientID);
        

        if (newPlayer != null)
        {
            CameraSetup_Rpc(newPlayer);
        }
        
        if (NetworkManager.Singleton.ConnectedClientsList.Count == maxPlayerCount)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;

            MenuClose_Rpc();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void MenuClose_Rpc()
    {
        canvasUI.SetActive(false);
    }
   

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void CameraSetup_Rpc(NetworkObjectReference playerRef)
    {
        if (playerRef.TryGet(out NetworkObject player))
        {
            if (player.IsLocalPlayer)
            {
                virtualCamera.Follow = player.transform;
            }
        }
    }
}