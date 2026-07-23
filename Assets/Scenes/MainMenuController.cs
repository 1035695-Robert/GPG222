using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : NetworkBehaviour
{
    [SerializeField] private GameObject joinlobbyUI;
    [SerializeField] private GameObject inLobbyUI;

[Rpc(SendTo.ClientsAndHost)]
    public void JoinedLobby_Rpc()
    {
        if(!IsClient) return;
        {
            Debug.Log("Joined Lobby");
            joinlobbyUI.SetActive(false);
            inLobbyUI.SetActive(true);
        }
    }
}