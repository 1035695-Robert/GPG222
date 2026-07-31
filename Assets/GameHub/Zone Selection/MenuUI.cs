using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MenuUI : NetworkBehaviour
{
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private List<GameObject> players = new List<GameObject>();

    private readonly NetworkVariable<bool> shouldShowUI = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        shouldShowUI.OnValueChanged += OnUIStateChange;
        levelSelection.SetActive(shouldShowUI.Value);
    }

    public override void OnNetworkDespawn()
    { 
        shouldShowUI.OnValueChanged -= OnUIStateChange;
    }

    private void OnUIStateChange(bool previousValue, bool newValue)
    {//updates automatically for all clients 
        levelSelection.SetActive(newValue);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        
        if (other.CompareTag("Player") && !players.Contains(other.gameObject))
        {//checks if the player is not already on the list before Adding
            players.Add(other.gameObject);
            CheckPlayerCount();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player") && players.Contains(other.gameObject))
        {//checks if the player that left is on the list allowing it to be removed from the list
            players.Remove(other.gameObject);
            CheckPlayerCount();
        }
    }

    private void CheckPlayerCount()
    {//checks if all players are on the space
        int count = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log("playerCount: " + players.Count + "/" + count);
        if (players.Count >= count)
        {//if all players are in the area toggle update the NetworkVariable
            shouldShowUI.Value = true;
        }
        else //if one player leave Update NetworkVariable
            shouldShowUI.Value = false;
    }
}