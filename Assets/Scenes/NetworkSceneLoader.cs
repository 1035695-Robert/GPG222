using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class NetworkSceneLoader : NetworkBehaviour
{
    private Dictionary<string, int> _selectedScene = new Dictionary<string, int>();
    private Lobby _lobby;

    public static NetworkSceneLoader Instance;
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
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GetLobby();
        }
    }

    private async void GetLobby()
    {
        try
        {
            _lobby =  await LobbyService.Instance.GetLobbyAsync(HostManager.Instance.lobbyId);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw; 
        }
    }

    public void OnLevelSelection(string sceneName)
    {
        if (IsClient)
        {
            Debug.Log("client");
            ButtonPressed_Rpc(sceneName);
        }
    }

    
    [Rpc(SendTo.Server)]
    private void ButtonPressed_Rpc(string sceneName)
    {
        if (_selectedScene.ContainsKey(sceneName))
        {
            _selectedScene[sceneName]++;
            Debug.Log(_selectedScene.Count);
            if (_selectedScene[sceneName] == _lobby.MaxPlayers)
            {
                _selectedScene.Clear();
                LoadScene(sceneName);
                
            }
        }
        else
        {
            _selectedScene.Add(sceneName, 1);
            
        }
        foreach (KeyValuePair<string, int> entry in _selectedScene)
        {
            Debug.Log($"Scene {entry.Key}: {entry.Value}//{_lobby.MaxPlayers}");
        }
    }


    private void LoadScene(string sceneName)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}