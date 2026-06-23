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

        public delegate void Pickup(NetworkObjectReference targeted, Vector3 targetPoint);
        public event Pickup OnPickupEvent;
        public event Action OnDroppedEvent;

        public event Action<RaycastHit> OnHandsEvent;
        
        [SerializeField] private LayerMask pickupLayerMask;

        [SerializeField] private float maxDistance;
        [SerializeField] private HoldableObject holdableObject;

        
        
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

        [SerializeField] private ConfigurableJoint playerJoint;
        [SerializeField] private bool isHolding;

        [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
        public void Interact_Rpc()
        {
            if (!isHolding) TryPickUp();
            else DropObject_Rpc();
        }

        private void TryPickUp()
        {
            if (!IsServer) return;
            Debug.Log("TryPickUp");
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                NetworkObjectReference target = hit.transform.root.gameObject;
                SetupPlayerJoint_Rpc(target);
                OnPickupEvent?.Invoke(target, hit.point);
                OnHandsEvent?.Invoke(hit);
            }
        }
      
        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
        void SetupPlayerJoint_Rpc(NetworkObjectReference targetRef)
        {
            if (targetRef.TryGet(out NetworkObject networkObject))
            {
                isHolding = true;
                GameObject target = networkObject.gameObject;

                playerJoint = gameObject.AddComponent<ConfigurableJoint>();
                playerJoint.connectedBody = target.GetComponent<Rigidbody>();
                playerJoint.anchor = Vector3.zero;

                playerJoint.connectedAnchor = transform.InverseTransformPoint(transform.forward); 

                playerJoint.xMotion = ConfigurableJointMotion.Free;
                playerJoint.yMotion = ConfigurableJointMotion.Free;
                playerJoint.zMotion = ConfigurableJointMotion.Free;

                playerJoint.angularXMotion = ConfigurableJointMotion.Locked;
                playerJoint.angularYMotion = ConfigurableJointMotion.Limited;
                playerJoint.angularZMotion = ConfigurableJointMotion.Locked;

                JointDrive xDrive = new JointDrive
                {
                    positionSpring = 1000f,
                    positionDamper = 50f,
                    maximumForce = Mathf.Infinity
                };
                playerJoint.xDrive = xDrive;

                JointDrive yDrive = new JointDrive
                {
                    positionSpring = 1000f,
                    positionDamper = 50f,
                    maximumForce = Mathf.Infinity
                };
                playerJoint.yDrive = yDrive;

                JointDrive zDrive = new JointDrive
                {
                    positionSpring = 1000f,
                    positionDamper = 50f,
                    maximumForce = Mathf.Infinity
                };
                playerJoint.zDrive = zDrive;
                
                playerJoint.enableCollision = true;
            }
        }
        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
        private void DropObject_Rpc()
        {
            Debug.Log("Dropping object");
            Destroy(playerJoint);
            
            OnDroppedEvent?.Invoke();
            isHolding = false;
        }
        
        
  
    }
}