using System.Collections.Generic;
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
            joinlobbyUI.SetActive(false);
            inLobbyUI.SetActive(true);
            //for (int i = 0; i < waitingPlayerUI.Length; i++)
         
    }
}

