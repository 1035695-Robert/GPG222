using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Random = UnityEngine.Random;


public class HostManager : NetworkBehaviour
{ 
    private CreateLobbyOptions _createLobbyOptions = new CreateLobbyOptions();

   
    [SerializeField] private string joinCode;
    
    [SerializeField]public string lobbyId;

    public string lobbyName;
    public int maxConnections;
    public bool privateState = false;
    public string passwordText;
    public static HostManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void StartHost()
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError($"relay create allocation failed{e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]}{allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.LogError($"relay get Join Code request failed{e.Message}");
            throw;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("udp"));

        await CreateLobby();
        
        NetworkManager.Singleton.StartHost();
    }


    private async Task CreateLobby()
    {
        try
        {
            _createLobbyOptions.IsPrivate = privateState;
            if (privateState == true)
            {
                _createLobbyOptions.Password = passwordText;
            }

            // Very spaced out creation of custom data for the lobby. In this the relay code (but could be lobby name, player count, map name etc)
            _createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxConnections, _createLobbyOptions);
            lobbyId = lobby.Id;


            if (lobby != null)
            {
                Debug.Log("CREATED LOBBY : " + lobby.Name);
            }

            // Heartbeat the lobby every 15 seconds.

            if (lobby != null)
                StartCoroutine(HeartbeatLobbyCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

}