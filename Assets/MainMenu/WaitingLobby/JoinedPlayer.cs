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
    private NetworkSceneLoader _sceneLoader;
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
        _sceneLoader = GameObject.Find("MenuManager").GetComponent<NetworkSceneLoader>();
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(Clicked);
        }

        foreach (var colourButton in bodyColourButtons)
        {
            colourButton.onClick.AddListener(() => BodyColour_Rpc(colourButton.gameObject.name));
        }
        // foreach (var colourButton in handColourButtons)
        // {
        //     colourButton.onClick.AddListener(() =>PlayerColourManager.Instance.BodyColour(colourButton.gameObject.name, OwnerClientId,"hand"));
        // }
    }

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
        displayScreen.color = newValue;
    }

    [Rpc(SendTo.Server)]
    private void SetBaseName_Rpc()
    {
        playerName.Value = "[ Player " + NetworkManager.Singleton.ConnectedClientsList.Count.ToString() + " ]";
    }


    [Rpc(SendTo.Server)]
    private void ChangeName_Rpc(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        UpdateNameDisplay_Rpc(newValue);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateNameDisplay_Rpc(FixedString128Bytes newValue)
    {
        displayName.text = newValue.ToString();
    }


    void Clicked()
    {
        joinButton.interactable = false;
        _sceneLoader.OnLevelSelection("GameHub");
    }

    [Rpc(SendTo.Server)]
    private void BodyColour_Rpc(string colour)
    {
        PlayerColourManager.Instance.BodyColour(colour, OwnerClientId, "body");
        PlayerColourManager.Instance.SetColourOnSceneLoad(OwnerClientId, "body");
    }
}