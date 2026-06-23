using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerView : NetworkBehaviour
    {
//         [SerializeField] private PlayerModel model;
//         [SerializeField] private GameObject playerHands;
//
//         private void OnEnable()
//         {
//             model.OnPickupEvent += GrabObject_Rpc;
//             model.OnDroppedEvent += DroppedObject;
//         }
//
//         private void OnDisable()
//         {
//             model.OnPickupEvent -= GrabObject_Rpc;
//             model.OnDroppedEvent -= DroppedObject;
//         }
//
//         [Rpc(SendTo.ClientsAndHost)]
//         void GrabObject_Rpc(NetworkObjectReference targetReference, Vector3 targetPoint)
//         {
//             if (targetReference.TryGet(out NetworkObject networkObject))
//             {
//                 GameObject targetObject = networkObject.gameObject;
//                 Debug.Log("Grab Object");
//                 playerHands.transform.SetParent(targetObject.transform);
//                 playerHands.transform.position = targetPoint + new Vector3(0, 0.25f, 0);
//             }
//         }
//
//
//         private void DroppedObject()
//         {
//             playerHands.transform.SetParent(transform.root);
//             transform.root.localPosition -= transform.forward * 0.1f;
//             playerHands.transform.localPosition = new Vector3(0, 0, 1f);
//         }
    }
}