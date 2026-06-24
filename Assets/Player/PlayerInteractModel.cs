using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractModel : NetworkBehaviour
{
    public delegate void Pickup(Vector3 targetPoint);

    public event Pickup OnPickupEvent;
    public event Action OnDroppedEvent;

    public event Action<RaycastHit> OnHandsEvent;

    [SerializeField] private LayerMask pickupLayerMask;

    [SerializeField] private float maxDistance;
    
    [SerializeField] private ConfigurableJoint playerJoint;
    [SerializeField] public NetworkVariable<bool> isHolding =  new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    public void Interact_Rpc()
    {
        if (!isHolding.Value) TryPickUp();
        else DropObject_Rpc();
    }

    private void TryPickUp()
    {
        Debug.Log("TryPickUp");
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
        {
            NetworkObjectReference target = hit.transform.root.gameObject;
            
            OnPickupEvent?.Invoke(hit.point);
            OnHandsEvent?.Invoke(hit);
            SetupPlayerJoint_Rpc(target);
        }
    }

    [SerializeField] private float driveSpringValue;

    void SetupPlayerJoint_Rpc(NetworkObjectReference targetRef)
    {
        if (targetRef.TryGet(out NetworkObject networkObject))
        {
           
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
                positionSpring = driveSpringValue,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            playerJoint.xDrive = xDrive;

            JointDrive yDrive = new JointDrive
            {
                positionSpring = Mathf.Infinity,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            playerJoint.yDrive = yDrive;

            JointDrive zDrive = new JointDrive
            {
                positionSpring = driveSpringValue,
                positionDamper = 50f,
                maximumForce = Mathf.Infinity
            };
            playerJoint.zDrive = zDrive;

            playerJoint.enableCollision = true;
            isHolding.Value = true;
        }
    }


    private void DropObject_Rpc()
    {
        Debug.Log("Dropping object");
        Destroy(playerJoint);

        OnDroppedEvent?.Invoke();
        isHolding.Value = false;
    }
}

