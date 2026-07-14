using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField]private string joinCode;
    [SerializeField] private string relayCode;
    
    
    public void StartHostWithRelay()
    {
      StartHostWithRelay(4, "udp");
    }

    public void StartClientWithRelay()
    {
        StartClientWithRelay(relayCode, "udp");
    }
    private async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData(connectionType));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private async Task<bool> StartClientWithRelay(string code, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: code);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData(connectionType));
        return !string.IsNullOrEmpty(code) && NetworkManager.Singleton.StartClient();
    }

    public void UserEnterRelayCode(string inputCode)
    {
        relayCode = inputCode;
    }
}
