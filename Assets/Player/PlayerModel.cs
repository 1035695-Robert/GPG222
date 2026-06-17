using Prefabs;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    //this is for server gameplay side
    public class PlayerModel : NetworkBehaviour
    {
        //private Vector3 _directionInput;

        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 1f;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Rigidbody playerRigidbody;
        public Vector2 moveValue;

        public delegate void Pickup(GameObject hands, GameObject targeted);

        public event Pickup OnPickup;

        public delegate void Drop(GameObject targetObject);

        public event Drop Dropped;

        [Header("hands")] [SerializeField] private GameObject hands;
        [SerializeField] private LayerMask pickupLayerMask;

        [SerializeField] private ConfigurableJoint playerPickupJoint;
        [SerializeField] private float maxDistance;
        [SerializeField] private HoldableObject holdableObject;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SpawnObjectRpc();

            
        }

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Unreliable)]
        public void SpawnObjectRpc()
        {
            Dropped?.Invoke(hands);
            playerPickupJoint.connectedBody = hands.gameObject.GetComponent<Rigidbody>();

        }
        private void Update()
        {
            if (IsOwner)
            {
                Vector3 directionInput = new Vector3(moveValue.x, 0, moveValue.y).normalized;

                Move(directionInput);
                Rotation(directionInput);
            }
        }


        private void Move(Vector3 directionInput)
        {
            playerRigidbody.MovePosition(playerRigidbody.position + directionInput * (walkSpeed * Time.deltaTime));
        }

        private void Rotation(Vector3 directionInput)
        {
            if (directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionInput, Vector3.up);
                playerRigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
        }


        public Rigidbody currentHoldRigidbody;
        private bool _isHolding;

        [Rpc(SendTo.Server)]
        public void Interact_Rpc()
        {
            if (!_isHolding) TryPickUp();
            else DropObject();
        }

        private void TryPickUp()
        {
            Debug.Log("TryPickUp");
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                GameObject targeted = hit.transform.gameObject;
                holdableObject = targeted.GetComponent<HoldableObject>();
                holdableObject.PickUp(hands);
                OnPickup?.Invoke(hands, targeted);
                
            }
        }

        private void DropObject()
        {
            if (!IsServer) return;
            Debug.Log("Dropping object");
            if (playerRigidbody != null)
            {
                playerPickupJoint.connectedBody = null;
            }
            
            Dropped?.Invoke(hands);
            
            playerPickupJoint.connectedBody = hands.gameObject.GetComponent<Rigidbody>();
            _isHolding = false;
        }
    }
}