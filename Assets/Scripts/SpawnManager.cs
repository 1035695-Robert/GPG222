using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] MainMenuController mainMenuController;

    [Header("PlayerManager")] [SerializeField]
    private GameObject canvasUI;

    public int maxPlayerCount = 2;

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
            SpawnPlayer();
        }
    }


    void SpawnPlayer()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject newPlayer = Instantiate(playerPrefab).GetComponent<NetworkObject>();
            newPlayer.SpawnAsPlayerObject(client.ClientId, true);
            if (SceneManager.GetActiveScene().name != "MainMenu")
                CameraSetup_Rpc(client.ClientId);
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
            
            NetworkManager.Singleton.SceneManager.LoadScene("GameHub", loadSceneMode: LoadSceneMode.Single);
        
    }


    private void SceneLoadCompleteHandler(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        Debug.Log("newScene");
        foreach (ulong clientID in clientsCompleted)
        {
            ConnectedClients(clientID);
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