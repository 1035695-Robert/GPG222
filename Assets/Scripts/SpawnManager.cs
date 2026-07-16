using Player.hands;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;

    [SerializeField] private HandsModel handsModel;
    [SerializeField] private GameObject playerHands;
    [SerializeField] private NetworkObject networkHands;


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
        HandSpawn_Rpc(clientID, newPlayer);

        if (newPlayer != null)
        {
            CameraSetup_Rpc(newPlayer);
        }
    }

    [Rpc(SendTo.Server)]
    void HandSpawn_Rpc(ulong clientID, NetworkObjectReference playerRef)
    {
        if (playerRef.TryGet(out NetworkObject player))
        {
            networkHands = Instantiate(playerHands).GetComponent<NetworkObject>();
            networkHands.SpawnWithOwnership(OwnerClientId);
            handsModel = networkHands.GetComponent<HandsModel>();
            networkHands.TrySetParent(player, false);
            handsModel.Setup();
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