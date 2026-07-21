using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject canvasUI;

    public int maxPlayerCount = 2;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void MenuClose_Rpc()
    {
        canvasUI.SetActive(false);
    }
}