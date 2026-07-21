using System.Collections.Generic;
using Player.hands;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;


    [Header("PlayerManager")] [SerializeField]
    private GameObject canvasUI;

    public int maxPlayerCount = 2;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoadCompleteHandler;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneLoadCompleteHandler;
        }
    }

    void SpawnPlayer(ulong clientID)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject != null) return;
        NetworkObject newPlayer = Instantiate(playerPrefab).GetComponent<NetworkObject>();
        newPlayer.SpawnAsPlayerObject(clientID,true);
        
        if (newPlayer != null)
        {
            CameraSetup_Rpc(clientID);
        }
        
        if (NetworkManager.Singleton.ConnectedClientsList.Count == maxPlayerCount)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;

            MenuClose_Rpc();
        }
    }

    private void SceneLoadCompleteHandler(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        foreach (ulong clientID in clientsCompleted)
        {
            SpawnPlayer(clientID);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void MenuClose_Rpc()
    {
        canvasUI.SetActive(false);
    }


    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void CameraSetup_Rpc(ulong clientID)
    {
      
            NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        
                if (player.IsLocalPlayer)
                {
                    virtualCamera.Follow = player.transform;
                }
                
    }
}