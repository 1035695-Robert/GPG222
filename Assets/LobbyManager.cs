using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;


public class LobbyManager : MonoBehaviour
{
    [SerializeField] private CreateLobbyOptions _options = new CreateLobbyOptions();
    [SerializeField] RelayManager relayManager;

    [SerializeField] private string lobbyName;
    [SerializeField] private int maxPlayers;
    [SerializeField] private string joinCode;

    public void HostLobby()
    {
        _ = CreateLobby();
    }

    public void ClientQuickJoin()
    {
        _ = QuickJoin();
    }

    private async Task CreateLobby()
    {
        await relayManager.StartHostWithRelay(maxPlayers, "udp");

        _options.IsPrivate = false;

        // Very spaced out creation of custom data for the lobby. In this the relay code (but could be lobby name, player count, map name etc)
        _options.Data = new Dictionary<string, DataObject>();
        DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, relayManager.joinCode,
            DataObject.IndexOptions.S1);
        _options.Data.Add("RelayCode", dataObject);


        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, _options);


        if (lobby != null)
        {
            Debug.Log("CREATED LOBBY : " + lobby.Name);
        }

        // Heartbeat the lobby every 15 seconds.

        if (lobby != null) StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
        else Debug.LogWarning("noLobby");
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
             //Debug.Log("Heartbeating lobby "+ Random.Range(0,1000));
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private async Task QuickJoin()
    {
        await relayManager.Initialise();
        try
        {
            // Quick-join a random lobby with a maximum capacity of 10 or more players.
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();


            // options.Filter = new List<QueryFilter>()
            //                  {
            //                      new QueryFilter(
            //                                      field: QueryFilter.FieldOptions.MaxPlayers,
            //                                      op: QueryFilter.OpOptions.GE,
            //                                      value: "10")
            //                  };


            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);


            if (lobby != null)
            {
                Debug.Log("JOINED LOBBY : " + lobby.Name);

                foreach (KeyValuePair<string, DataObject> keyValuePair in lobby.Data)
                {
                    Debug.Log("Key = " + keyValuePair.Key);
                    Debug.Log("Value = " + keyValuePair.Value.Value);
                }


                await relayManager.StartClientWithRelay(lobby.Data["RelayCode"].Value, "udp");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}