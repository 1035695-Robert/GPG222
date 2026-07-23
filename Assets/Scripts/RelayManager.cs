using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public int playerCount = 2;
    [SerializeField] public string joinCode;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    [SerializeField] private string relayCode;


    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject hostUi;
    [SerializeField] private GameObject clientUi;

    public  void StartHostWithRelay()
    {
         _= StartHostWithRelay(playerCount, "udp");
    }

    public void JoinUI()
    {
        clientUi.SetActive(true);
        mainUI.SetActive(false);
    }

    public void StartClientWithRelay()
    {
        _ = Initialise();
        _ = StartClientWithRelay(relayCode, "udp");
    }

    public async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
    {
        await Initialise();

        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetRelayServerData(allocation.ToRelayServerData(connectionType));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    void HostMenu()
    {
        hostUi.SetActive(true);
        joinCodeText.text = joinCode;
        mainUI.SetActive(false);
    }

    public async Task Initialise()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async Task<bool> StartClientWithRelay(string code, string connectionType)
    {
      //  await Initialise();
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: code);
        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetRelayServerData(allocation.ToRelayServerData(connectionType));
        return !string.IsNullOrEmpty(code) && NetworkManager.Singleton.StartClient();
    }

    public void MainMenu()
    {
        mainUI.SetActive(true);
        if (clientUi)
        {
            clientUi.SetActive(false);
        }
    }

    public void UserEnterRelayCode(string inputCode)
    {
        relayCode = inputCode;
    }
}