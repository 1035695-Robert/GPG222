using System;
using Player;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinedPlayer : NetworkBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private Button[] bodyColourButtons;
    [SerializeField] private Button[] handColourButtons;
    [SerializeField] private Button leaveButton;

    [SerializeField] private Image displayScreen;

    [SerializeField] private PlayerColourModel colourState;

    //networkVariables can not be strings use FixedStrings then convert .ToString to update TMPro
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("[Player]",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] TextMeshProUGUI displayName;

    public override void OnNetworkSpawn()
    {
        ChangeColourServer_Rpc(colourState.networkBodyColour.Value);
        if (!IsOwner) return;

        playerName.OnValueChanged += ChangeName_Rpc;
        
        colourState.networkBodyColour.OnValueChanged += OnColourChange;
        SetBaseName_Rpc();
        joinButton.interactable = true;
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(Clicked);
        }

        leaveButton.interactable = true;
        leaveButton.onClick.AddListener(LeaveLobby);

        foreach (var colourButton in bodyColourButtons)
        {
            colourButton.interactable = true;
            colourButton.onClick.AddListener(() => BodyColour_Rpc(colourButton.gameObject.name));
        }
        //removed hands due to the colour were not setting when joined. this could be because it was being called before the Hands were instantiated into scene;

        // foreach (var colourButton in handColourButtons)
        // {
        //     colourButton.onClick.AddListener(() =>PlayerColourManager.Instance.BodyColour(colourButton.gameObject.name, OwnerClientId,"hand"));
        // }
    }

    private void LeaveLobby()
    {
        //notifies the Lobby to disconnect the player
        LobbyDisconnectionManager.Instance.LeaveLobby();
    }

    #region Colour Change

    private void OnColourChange(Color previousValue, Color newValue)
    {
        ChangeColourServer_Rpc(newValue);
    }

    [Rpc(SendTo.Server)]
    private void ChangeColourServer_Rpc(Color newValue)
    {
        ChangeColour_Rpc(newValue);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeColour_Rpc(Color newValue)
    {
        //updates the display panel colour in waiting lobby for the Client
        displayScreen.color = newValue;
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void BodyColour_Rpc(string colour)
    {
        //sends signal to update the NetworkVariable located in PlayerColourModel
        PlayerColourManager.Instance.BodyColour(colour, OwnerClientId, "body");

        //sends signal to update the UI visual colour.
        PlayerColourManager.Instance.SetColourOnSceneLoad(OwnerClientId, "body");
    }

    #endregion

    #region Name Dislay

    //server has authority to write the NetworkVariable
    [Rpc(SendTo.Server)]
    private void SetBaseName_Rpc()
    {
        //this was implimented originally to give the player customisable Name when they joined the lobby,
        playerName.Value = "[ Player " + NetworkManager.Singleton.ConnectedClientsList.Count.ToString() + " ]";
    }


    [Rpc(SendTo.Server)]
    private void ChangeName_Rpc(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        UpdateNameDisplay_Rpc(newValue);
    }

    //updates for all clients
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateNameDisplay_Rpc(FixedString128Bytes newValue)
    {
        displayName.text = newValue.ToString();
    }

    #endregion

    void Clicked()
    {
        //locks the player in ready to play when all other players are ready
        joinButton.interactable = false;
        foreach (var colourButton in bodyColourButtons)
        {
            colourButton.interactable = false;
        }

        NetworkSceneLoader.Instance.ButtonPressed_Rpc("GameHub");
    }
}