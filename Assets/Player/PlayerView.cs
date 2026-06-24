using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerView : NetworkBehaviour
    {
        [FormerlySerializedAs("model")] [SerializeField] private PlayerMovementModel movementModel;

        // public override  void OnNetworkSpawn()
        // {   
        //     model.OnPickupEvent += GrabObject_Rpc;
        //     model.OnDroppedEvent += DroppedObject_Rpc;
        // }
        //
        // public override  void OnNetworkDespawn()
        // {
        //     model.OnPickupEvent -= GrabObject_Rpc;
        //     model.OnDroppedEvent -= DroppedObject_Rpc;
        // }
        //
        // [Rpc(SendTo.ClientsAndHost)]
        // void GrabObject_Rpc(Vector3 targetPoint)
        // {
        //     
        // }
        //
        // [Rpc(SendTo.ClientsAndHost)]
        // private void DroppedObject_Rpc()
        // {
        //  
        // }
    }
}