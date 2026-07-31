using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyDisconnectionManager : NetworkBehaviour
{
    public static LobbyDisconnectionManager Instance;

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


    [SerializeField] private string _localPlayerID;
    [SerializeField] private string _currentLobbyId;

    //when players join both variables are stored here allowing the server to know
    //who(player) and where(lobby) they are trying to leave the Lobby

    public void SetPlayerID(string playerID) => _localPlayerID = playerID;
    public void SetLobbyID(string lobbyID) => _currentLobbyId = lobbyID;

    public async void LeaveLobby()
    {
        if (NetworkManager.Singleton.IsHost && _currentLobbyId != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_currentLobbyId);
                Debug.Log("Lobby deleted successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete lobby:{e}");
                throw;
            }
            finally
            {
                ShutDown();
            }
        }

        else if (!string.IsNullOrEmpty(_currentLobbyId) && !string.IsNullOrEmpty(_localPlayerID))
        {
            //checks to see if the client and lobby actually exist 
            try
            {
                // removes the client from the Lobby 
                await LobbyService.Instance.RemovePlayerAsync(_currentLobbyId, _localPlayerID);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"failed to leave lobby {_localPlayerID}: {e.Message}");
                throw;
            }
            finally
            {
                ShutDown();
            }
        }
    }

    private void ShutDown()
    {
        //Shutting down the NetworkManager on the Clients side before being returned to the MainMenu
        //doing this allows for the player to have no connection to any lobby allowing them to join another lobby
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }


    public void ClientDisconnectedHandler(ulong clientId)
    {
        //Disconnects the client from the NetworkManager and takes them back to the MainMenu
        //Helps with managing sudden disconnection.
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }
    }

//used to stop the game from timing out with the a networkTransport failure
// this just also helps to not Reboot the Game in unity as sends them to the main menu 
// if it wasnt here it would disconnect the player/s and leave a ghost scene (no players)
    public void NetworkTransportFailed()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}