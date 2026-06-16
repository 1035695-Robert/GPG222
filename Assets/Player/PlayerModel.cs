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
            Movement_Rpc();
            Rotation_Rpc();
        }
       [Rpc(SendTo.ClientsAndHost)]
        private void Movement_Rpc()
        {
            _directionInput = new Vector3(moveValue.x, 0, moveValue.y);
            _rigidbody.MovePosition(_rigidbody.position + _directionInput * (walkSpeed * Time.deltaTime));
        }
        [Rpc(SendTo.ClientsAndHost)]
        private void Rotation_Rpc()
        {
            if (_directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_directionInput);
                _rigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
        }

       
        public Rigidbody currentHoldRigidbody;
        private bool _isHolding;
       
        public void Interact()
        {
            if (!_isHolding) TryPickUp_Rpc();
            else DropObject_Rpc();
        }

        
        private void TryPickUp_Rpc()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                currentHoldRigidbody = hit.rigidbody;
                currentHoldRigidbody.isKinematic = false;
               _playerPickupJoint.connectedBody = currentHoldRigidbody;
               _playerPickupJoint.xMotion = ConfigurableJointMotion.Locked;
               _playerPickupJoint.yMotion = ConfigurableJointMotion.Locked;
               _playerPickupJoint.zMotion = ConfigurableJointMotion.Locked;
               _playerPickupJoint.angularXMotion = ConfigurableJointMotion.Locked;
               _playerPickupJoint.angularYMotion = ConfigurableJointMotion.Limited;
               _playerPickupJoint.angularZMotion = ConfigurableJointMotion.Locked;
               
               _isHolding = true;
            }
        }
       
      
        private void DropObject_Rpc()
        {
            Debug.Log("Dropping object");
            _playerPickupJoint.xMotion = ConfigurableJointMotion.Free;
            _playerPickupJoint.yMotion = ConfigurableJointMotion.Free;
            _playerPickupJoint.zMotion = ConfigurableJointMotion.Free;
            _playerPickupJoint.angularXMotion = ConfigurableJointMotion.Free;
            _playerPickupJoint.angularYMotion = ConfigurableJointMotion.Free;
            _playerPickupJoint.angularZMotion = ConfigurableJointMotion.Free;
                currentHoldRigidbody.isKinematic = true;
                _playerPickupJoint.connectedBody = null;
                currentHoldRigidbody = null;
                _isHolding = false;
        }

        public void UpdateMoveValue(Vector2 value)
        {
           moveValue = value;
        }
    }
}