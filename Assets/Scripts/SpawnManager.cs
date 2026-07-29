using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Camera[] rendererCameras;

    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("PlayerManager")] [SerializeField]
    private GameObject canvasUI;

    public int maxPlayerCount = 4;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedClients;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoadCompleteHandler;
    }

    private void ConnectedClients(ulong clientID)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject != null) return;
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= maxPlayerCount)
        {
            SpawnPlayer_Rpc();
        }
    }
    private void SceneLoadCompleteHandler(string sceneName, LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        Debug.Log("newScene");
        foreach (ulong clientID in clientsCompleted)
        {
            ConnectedClients(clientID);
        }
    }
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int usedCount;

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    void SpawnPlayer_Rpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            //when loading new scene it will create new player characters and spawn them on their own designated Spawn points
            NetworkObject newPlayer = Instantiate(playerPrefab).GetComponent<NetworkObject>();

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (i == usedCount)
                {
                    usedCount++;
                    newPlayer.transform.position = spawnPoints[i].position;
                    break;
                }
            }

            newPlayer.SpawnAsPlayerObject(client.ClientId, true);
            PlayerColourManager.Instance.SetColourOnSceneLoad(client.ClientId, "body");
            if (virtualCamera != null) CameraSetup_Rpc(client.ClientId);
        }
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

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ConnectedClients;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneLoadCompleteHandler;
        }
    }
}