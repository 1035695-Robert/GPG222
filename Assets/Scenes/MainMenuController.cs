using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject customisationUI;


    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    public void CustomisationMenu_Rpc()
    {
        if (IsServer)
            lobbyUI.SetActive(false);
        customisationUI.SetActive(true);
    }


    private void LoadLevel()
    {
        if (IsHost)
        {
            //loadLevel
        }
    }
}