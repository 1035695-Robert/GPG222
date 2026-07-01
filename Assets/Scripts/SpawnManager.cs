using JetBrains.Annotations;
using Prefabs.EndGoal;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;
    
    [SerializeField] private EndGoalController endGoalController;
    
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