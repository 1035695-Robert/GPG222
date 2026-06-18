// using Unity.Netcode.Components;
// using UnityEngine;
//
// public class PickupObject : InteractObject
// {
//    [SerializeField] private ComponentController controller;
//     protected override void ApplyAvailabilityState(bool newValue)
//     {
//         if(IsServer)
//         {
//             
//         }
//     }
//
//     protected override void OnPickedUp()
//     {
//        // no code
//     }
//     
//     public void Drop(Vector3 position)
//     {
//         if (!IsServer) return;
//         transform.position = new Vector3(position.x, transform.position.y, position.z);
//         IsAvailable.Value = true;
//     }
//     
// }
