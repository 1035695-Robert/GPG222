using System;
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

        public delegate void Pickup(GameObject targeted, Vector3 targetPoint, NetworkObject networkHands);
        public delegate void Drop(NetworkObject networkHands);
        public event Pickup OnPickupEvent;
        public event Drop OnDroppedEvent;
        
        [SerializeField] private LayerMask pickupLayerMask;

        [SerializeField] private float maxDistance;
        [SerializeField] private HoldableObject holdableObject;

        [SerializeField] private GameObject playerHandPrefab;
        private NetworkObject _networkHands;
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
            if (isHolding) return;
            if (directionInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionInput, Vector3.up);
                playerRigidbody.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime * 5f);
            }
            else
                playerRigidbody.angularVelocity = Vector3.zero;
        }

        [SerializeField] private ConfigurableJoint joint;
        [SerializeField] private bool isHolding;

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
        public void Interact_Rpc()
        {
            if (!isHolding) TryPickUp();
            else DropObject_Rpc();
        }

        private void TryPickUp()
        {
            Debug.Log("TryPickUp");
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                GameObject target = hit.transform.root.gameObject;
                SetupJoint_Rpc();
                joint.connectedBody = target.GetComponent<Rigidbody>();
                OnPickupEvent?.Invoke(hit.transform.gameObject, hit.point, _networkHands);
                isHolding = true;
            }
        }

      
        void SetupJoint_Rpc()
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            
            joint.anchor = Vector3.zero;
            
            joint.connectedAnchor = transform.InverseTransformPoint(transform.forward);

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            JointDrive xDrive = new JointDrive
            {
                positionSpring = 1000f,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            joint.xDrive = xDrive;

            JointDrive yDrive = new JointDrive
            {
                positionSpring = 1000f,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            joint.yDrive = yDrive;

            JointDrive zDrive = new JointDrive
            {
                positionSpring = 1000f,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            joint.zDrive = zDrive;
        }


        
        private void DropObject_Rpc()
        {
            Debug.Log("Dropping object");
            Destroy(joint);
            OnDroppedEvent?.Invoke(_networkHands);
            isHolding = false;
        }
        [Rpc(SendTo.Server)]
        public void SpawnHands_Rpc()
        {
            GameObject hands = Instantiate(
                playerHandPrefab,
                new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.75f),
                transform.rotation);
            _networkHands = hands.GetComponent<NetworkObject>();
            _networkHands.Spawn();

            bool success = _networkHands.TrySetParent(gameObject.transform);
            if (!success)
            {
                Debug.LogError("failed");
            }
        }
    }
}