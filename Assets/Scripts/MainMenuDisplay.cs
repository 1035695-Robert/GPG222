using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private TMP_InputField joinCodeInputField;

    public static MainMenuDisplay Instance;

    private void Awake()
    {//singleton allowing it to be called without a direct reference
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

    private async void Start()
    {//initialises the Player when they start the game allowing them to connect to a lobby 
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            //disconnectionManager need reference to PlayerID to handle the disconnenctions
            LobbyDisconnectionManager.Instance.SetPlayerID(AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public async Task<bool> StartHost()
    {
        await HostManager.Instance.StartHost();
        return true;
    }

    public async void StartClient()
    {
        await ClientManager.Instance.JoinPrivateClient(joinCodeInputField.text);
    }
}