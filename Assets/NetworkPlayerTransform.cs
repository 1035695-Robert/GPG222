using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerTransform : NetworkBehaviour
{
   //what do I need to network
   
   //y-axis rotation
   private float _networkRotation;
   // position X and z axis 
   private Vector3 _networkPosition;
   //private Movement
   private Vector3 _networkVelocity;
   
    private void updatePositionPhysics_Rpc(Vector3 position, Vector3 linearVelocity,float angularVelocity )
    {
        
    }
}
