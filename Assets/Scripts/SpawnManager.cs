using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject playerSprite;
    [SerializeField] private Camera[] rendererCameras;

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

    [SerializeField] private Transform[] spawnPoints;
    private int spriteCount;

    [SerializeField] private int usedCount;

    void SpawnPlayer()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //starts game before the characters can spawn
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject sprite = Instantiate(playerSprite).GetComponent<NetworkObject>();


                mainMenuController.JoinedLobby_Rpc();
                
                sprite.SpawnAsPlayerObject(client.ClientId, true);
                CameraSprite_Rpc(client.ClientId);
                


                // NetworkManager.Singleton.SceneManager.LoadScene("GameHub", loadSceneMode: LoadSceneMode.Single);
            }

            return;
        }

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
            if (SceneManager.GetActiveScene().name != "MainMenu")
                CameraSetup_Rpc(client.ClientId);
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

    private int displayCount;
    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void CameraSprite_Rpc(ulong clientID)
    {
        NetworkObject sprite = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        
            for (int i = 0; i < rendererCameras.Length; i++)
            {
                if (i == displayCount)
                {
                    
                    
                }
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