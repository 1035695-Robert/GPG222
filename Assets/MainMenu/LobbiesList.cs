using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;
    private bool isRefreshing;
    private bool isJoining;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;

        isRefreshing = true;
       //while toggles on it refreshes the lineUp of Lobbies that can be joined
       //this will toggle on and off to prevent it from looping.
        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = 12;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
               
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };
            var lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyInstance.Initialise(this, lobby);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        try
        {
            var joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientManager.Instance.StartClient(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isJoining = false;
            throw;
        }
        isRefreshing = false;
    }
}