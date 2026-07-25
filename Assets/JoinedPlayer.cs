using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinedPlayer : NetworkBehaviour
{
  [SerializeField] private Button joinButton;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        joinButton.interactable = true;
        
    }
}
