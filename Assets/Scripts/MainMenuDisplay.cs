using System;
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

    

    

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {   
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void StartHost()
    {
        HostManager.Instance.StartHost();
    }

    public async void StartClient()
    {
        await ClientManager.Instance.StartClient(joinCodeInputField.text);
    }
}