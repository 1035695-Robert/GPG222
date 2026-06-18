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

        // public delegate void Pickup(GameObject hands, GameObject targeted);
        //
        // public event Pickup OnPickup;
        //
        // public delegate void Drop(GameObject targetObject);
        //
        // public event Drop Dropped;

        [Header("hands")] [SerializeField] private GameObject hands;
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
        }

        [SerializeField] private ConfigurableJoint joint;
        [SerializeField] private bool isHolding;

        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
        public void Interact_Rpc()
        {
            if (!isHolding) TryPickUp();
            else DropObject();
        }

        private void TryPickUp()
        {
            Debug.Log("TryPickUp");
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
            {
                GameObject target = hit.transform.root.gameObject;
                SetupJoint(target);

                isHolding = true;
            }
        }

        void SetupJoint(GameObject target)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = target.GetComponent<Rigidbody>();
            
            joint.anchor = Vector3.zero;
            
            //Vector3 worldTargetAnchor = target.transform.TransformPoint(new Vector3(0f, 0f, 1.5f));

           
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


        private void DropObject()
        {
            Debug.Log("Dropping object");

            Destroy(joint);

            isHolding = false;
        }
    }
}