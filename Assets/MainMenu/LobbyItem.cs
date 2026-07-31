using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text LobbyPlayerName;
    private LobbiesList  lobbiesList;
    private  Lobby lobby;

    public void Initialise(LobbiesList lobbiesList, Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        LobbyPlayerName.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public async void JoinLobby()
    {
        await lobbiesList.JoinAsync(lobby);
        transform.root.Find("LobbiesList").gameObject.SetActive(false);
    }
}