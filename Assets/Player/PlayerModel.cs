using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerModel : NetworkBehaviour
    {
        private Rigidbody _rigidbody;
        private Vector3 _directionInput;

        [Header("movement speed values")] [SerializeField]
        private float walkSpeed = 1f;

        [SerializeField] private float rotationSpeed = 5f;

        public Vector2 moveValue;

        //public delegate void Pickup();
        //public static event Pickup OnPickup;

        [SerializeField] private LayerMask pickupLayerMask;

        private ConfigurableJoint _playerPickupJoint;
        [SerializeField] private float maxDistance;

        public override void OnNetworkSpawn()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerPickupJoint = GetComponent<ConfigurableJoint>();
        }

        private void FixedUpdate()
        {
            Movement();
            Rotation();
        }

        private void Movement()
        {
            _directionInput = new Vector3(moveValue.x, 0, moveValue.y);
            _rigidbody.MovePosition(_rigidbody.position + _directionInput * (walkSpeed * Time.deltaTime));
        }

        private void Rotation()
        {
            if (_directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_directionInput);
                _rigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
        }

        private ConfigurableJoint _currentHoldJoint;
        public Rigidbody _currentHoldRigidbody;
        private bool _isHolding;

        public void Interact()
        {
            if (!_isHolding) TryPickUpRpc();
            else DropObject();

        }

        [Rpc(SendTo.Server)]
        private void TryPickUpRpc()
        {
           
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                _currentHoldRigidbody = hit.rigidbody;
                _currentHoldRigidbody.isKinematic = false;
               _playerPickupJoint.connectedBody = _currentHoldRigidbody;
               _isHolding = true;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientGrabRpc(ulong objectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject net))
            {
                ConfigurableJoint joint = net.GetComponent<ConfigurableJoint>();
                joint.connectedBody = transform.GetComponent<Rigidbody>();
            }
             
        }

        private void DropObject()
        {
            Debug.Log("Dropping object");
                _currentHoldRigidbody.isKinematic = true;
                _playerPickupJoint.connectedBody = null;
                _currentHoldRigidbody = null;
                _currentHoldJoint = null;
                _isHolding = false;
        }
    }
}