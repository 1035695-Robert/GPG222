using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerView : NetworkBehaviour
    {
        [SerializeField] private PlayerModel model;

        //[SerializeField] private NetworkObject playerHands;


        private void OnEnable()
        {
            model.OnPickupEvent += GrabObject;
            model.OnDroppedEvent += DroppedObject;
        }

        private void OnDisable()
        {
            model.OnPickupEvent -= GrabObject;
            model.OnDroppedEvent -= DroppedObject;
        }

        void GrabObject(GameObject targetObject, Vector3 targetPoint, NetworkObject playerHands)
        {
            Debug.Log("Grab Object");
            playerHands.TrySetParent(targetObject);
            playerHands.transform.position = targetPoint + new Vector3(0, 0.5f, 0);
        }


        private void DroppedObject(NetworkObject playerHands)
        {
           

           bool isPlayer = playerHands.TrySetParent(gameObject);
           if(!isPlayer)
              Debug.LogError("No hands"); 
        }
    }
}