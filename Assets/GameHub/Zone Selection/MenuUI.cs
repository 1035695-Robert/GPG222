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
    {
        levelSelection.SetActive(newValue);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player") && !players.Contains(other.gameObject))
        {
            players.Add(other.gameObject);
            CheckPlayerCount();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player") && players.Contains(other.gameObject))
        {
            players.Remove(other.gameObject);
            CheckPlayerCount();
        }
    }

    private void CheckPlayerCount()
    {
        int count = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log("playerCount: " + players.Count + "/" + count);
        if (players.Count >= count)
        {
            shouldShowUI.Value = true;
        }
        else
            shouldShowUI.Value = false;
    }
}