using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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


    [SerializeField] private string joinRelayCode;

    [SerializeField] private string lobbyCode;
    public TextMeshProUGUI joinCodeText;
    [SerializeField] public string lobbyId;

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

    public async Task StartHost()
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

        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            joinRelayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
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
          

            // Very spaced out creation of custom data for the lobby. In this the relay code (but could be lobby name, player count, map name etc)
            _createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinRelayCode
                    )
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxConnections, _createLobbyOptions);
            lobbyId = lobby.Id;
            lobbyCode = lobby.LobbyCode;
            if (privateState == true)
            {
                joinCodeText.text = "Code: " + lobbyCode;
            }
            LobbyDisconnectionManager.Instance.SetLobbyID(lobbyId);

            Debug.Log("CREATED LOBBY : " + lobby.Name);
            // Heartbeat the lobby every 15 seconds.
            StartCoroutine(HeartbeatLobbyCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(float waitTimeSeconds)
    { // Prevents the host from disconnection/timeOuts
        var delay = new WaitForSeconds(waitTimeSeconds);
        while (true)
        { //sends signals to lobby to prevent from becoming inactive
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    
}