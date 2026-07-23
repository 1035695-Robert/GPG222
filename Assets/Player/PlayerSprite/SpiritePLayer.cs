using Unity.Netcode;
using UnityEngine;

public class SpiritePLayer : NetworkBehaviour
{
    [SerializeField] RenderTexture[] rendererCameras;

    [SerializeField] private Camera cameraDisplay;

    public override void OnNetworkSpawn()
    {
        
            cameraDisplay.targetTexture = rendererCameras[0];
        
            cameraDisplay.targetTexture = rendererCameras[1];
    }
}