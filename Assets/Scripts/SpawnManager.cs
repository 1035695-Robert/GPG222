using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;

    [SerializeField] private Vector3 spawnPoints;
    [SerializeField] private float spawnLength;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedClients;
        NetworkManager.Singleton.OnClientDisconnectCallback += LobbyDisconnectionManager.Instance.ClientDisconnectedHandler;
        NetworkManager.Singleton.OnTransportFailure += LobbyDisconnectionManager.Instance.NetworkTransportFailed;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoadCompleteHandler;
    }


// handles the Game when New Scene has Loaded this allows the players to be spawned into the scenes
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

    private void ConnectedClients(ulong clientID)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject != null) return;

        SpawnPlayer_Rpc();
    }


    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    void SpawnPlayer_Rpc()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        float spacing = spawnLength / (playerCount - 1);

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            //when loading new scene it will create new player characters and spawn them on their own designated Spawn points
            NetworkObject newPlayer = Instantiate(playerPrefab).GetComponent<NetworkObject>();

            #region Spawn Location

            for (int i = 0; i < playerCount; i++)
            {
                if ((int)client.ClientId == i)
                {
                    //spawns players in intervals to space the players out, depending on how many players are connected
                    Vector3 spawnPosition = spawnPoints + new Vector3(i * spacing, 0, 0);
                    newPlayer.transform.position = spawnPosition;
                    break;
                }
            }

            #endregion

            //Spawns and connects the player 
            newPlayer.SpawnAsPlayerObject(client.ClientId, true);
            //updates the players Body colour
            PlayerColourManager.Instance.SetColourOnSceneLoad(client.ClientId, "body");


            if (virtualCamera != null) CameraSetup_Rpc(client.ClientId);
        }
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void CameraSetup_Rpc(ulong clientID)
    {
        NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        //applies the cinemachine to the player to follow only on the local player in scene for each player
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
            NetworkManager.Singleton.OnClientDisconnectCallback -= LobbyDisconnectionManager.Instance.ClientDisconnectedHandler;
            NetworkManager.Singleton.OnTransportFailure -= LobbyDisconnectionManager.Instance.NetworkTransportFailed;
        }
    }
}