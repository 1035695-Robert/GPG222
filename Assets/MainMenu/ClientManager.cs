using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;

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


    public async Task JoinPrivateClient(string inputLobbyCode)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(inputLobbyCode);
            Debug.Log($"joined private lobby: {lobby.Id}");

            string joinCode = lobby.Data["JoinCode"].Value;

            await StartClient(joinCode, lobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    //reason for using LobbyCode to Join instead of Relay, was that the disconnectionManager was able to use the LobbyID
    // reference is shown on line 57

    public async Task StartClient(string joinCode, string lobbyId)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            //used by the Lobbyitem and JoinPrivate client Join call.
            LobbyDisconnectionManager.Instance.SetLobbyID(lobbyId);
            
        }
        catch (Exception e)
        {
            Debug.Log($"relay get join code request failed{e.Message}");
            throw;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("udp"));

        NetworkManager.Singleton.StartClient();
    }
}