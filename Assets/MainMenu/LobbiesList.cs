using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            options.Count = 12; // the Count of how many RESULTS/LOBBIES to return.

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots, // the Name of the field to filter on
                    op: QueryFilter.OpOptions.GT, // the operator used to compare the field to the filter value
                    value: "0"), //the value to comp[are to the filed being filtered

                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked, //
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
                //spawns in a lobby item for each lobby that is Public and displaying it in the List
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

    public async Task JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;

        try     
        {
            var joinLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joinLobby.Data["JoinCode"].Value;
            //sends a request for the Client to join the Lobby and waits to be allowed to join.
            await ClientManager.Instance.StartClient(joinCode, joinLobby.Id);
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