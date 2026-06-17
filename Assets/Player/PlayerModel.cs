using Unity.Netcode;
using UnityEngine;

namespace Player
{
    //this is for server gameplay side
    public  class PlayerModel : NetworkBehaviour
    {
        //private Vector3 _directionInput;

        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 1f;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Rigidbody playerRigidbody;
        public Vector2 moveValue;

        //public delegate void Pickup();
        //public static event Pickup OnPickup;
        [Header("Pickup variables")] [SerializeField]
        private LayerMask pickupLayerMask;

        [SerializeField] private ConfigurableJoint playerPickupJoint;
        [SerializeField] private float maxDistance;
        

        private void Update()
        {
            if (IsOwner)
            {
                Vector3 directionInput = new Vector3(moveValue.x, 0, moveValue.y).normalized;

                MoveRpc(directionInput);
                RotationRpc(directionInput);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void MoveRpc(Vector3 directionInput)
        {
            if (IsOwner)
            {
                playerRigidbody.MovePosition(playerRigidbody.position + directionInput * (walkSpeed * Time.deltaTime));
            }
        }

        private void RotationRpc(Vector3 directionInput)
        {
            if (directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionInput);
                playerRigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
        }


        public Rigidbody currentHoldRigidbody;
        private bool _isHolding;

        public void Interact()
        {
            if (!_isHolding) TryPickUp();
            else DropObject_Rpc();
        }
        
        private void TryPickUp()
        {Debug.Log("TryPickUp");
            if (IsOwner)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
                {
                    if (hit.collider.TryGetComponent(out NetworkObject networkObject))
                    {
                       ServerHoldObject_Rpc(networkObject.NetworkObjectId);
                    }
                    //  
                    //  NetworkObject holdObject = hit.collider.GetComponent<NetworkObject>();
                    //
                    //
                    // _isHolding = true;
                    //
                }
            }
        }

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
        private void ServerHoldObject_Rpc(ulong holdObject)
        {
            ClientHoldObject_Rpc(holdObject);
        }

        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
        private void ClientHoldObject_Rpc(ulong holdObject)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(holdObject, out var networkObject))
            {
                if (networkObject != null)
                {
                    GameObject holdingObject = networkObject.gameObject;
                    currentHoldRigidbody = holdingObject.GetComponent<Rigidbody>();
                }


            }
        }
        // currentHoldRigidbody.isKinematic = false;
          
            // playerPickupJoint.xMotion = ConfigurableJointMotion.Locked;
            // playerPickupJoint.yMotion = ConfigurableJointMotion.Locked;
            // playerPickupJoint.zMotion = ConfigurableJointMotion.Locked;
            // playerPickupJoint.angularXMotion = ConfigurableJointMotion.Locked;
            // playerPickupJoint.angularYMotion = ConfigurableJointMotion.Limited;
            // playerPickupJoint.angularZMotion = ConfigurableJointMotion.Locked;
        

        private void DropObject_Rpc()
        {
            Debug.Log("Dropping object");
            // playerPickupJoint.xMotion = ConfigurableJointMotion.Free;
            // playerPickupJoint.yMotion = ConfigurableJointMotion.Free;
            // playerPickupJoint.zMotion = ConfigurableJointMotion.Free;
            // playerPickupJoint.angularXMotion = ConfigurableJointMotion.Free;
            // playerPickupJoint.angularYMotion = ConfigurableJointMotion.Free;
            // playerPickupJoint.angularZMotion = ConfigurableJointMotion.Free;
           // currentHoldRigidbody.isKinematic = true;
            playerPickupJoint.connectedBody = null;
           currentHoldRigidbody = null;
            _isHolding = false;
        }
    }
}