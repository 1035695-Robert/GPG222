using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerView : NetworkBehaviour
    {
        
        [SerializeField] private PlayerModel model;

        // private void OnEnable()
        // {
        //     model.OnPickup += GrabObject;
        //     model.Dropped += DroppedObject;
        // }
        //
        // private void OnDisable()
        // {
        //     model.OnPickup -= GrabObject;
        //     model.Dropped -= DroppedObject;
        // }
        //
        // private void GrabObject(GameObject hands, GameObject targetObject)
        // {
        //     Debug.Log("Grab Object");
        //     hands.transform.SetParent(targetObject.transform);
        // }
        //
        //
        // private void DroppedObject(GameObject targetObject)
        // {
        //     if (IsServer)
        //     {
        //         Debug.Log("Dropped Object");
        //         targetObject.transform.SetParent(transform);
        //     }
        // }
    }
}