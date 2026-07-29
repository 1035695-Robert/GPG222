using System;
using Unity.Netcode;
using UnityEngine;

public class ColourSelection : NetworkBehaviour
{
   

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        
    }
}
