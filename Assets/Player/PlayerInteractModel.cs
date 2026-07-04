using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractModel : NetworkBehaviour
{
    public delegate void Pickup();

    public event Pickup OnPickupEvent;
    public event Action OnDroppedEvent;
    public event Action<RaycastHit> OnHandsEvent;

    [SerializeField] private LayerMask pickupLayerMask;

    [SerializeField] private float maxDistance;

    [SerializeField] private ConfigurableJoint playerJoint;

    [SerializeField] public NetworkVariable<bool> isHolding = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    public void Interact_Rpc()
    {
        if (!isHolding.Value) TryPickUp();
        else DropObject();
    }

    private void TryPickUp()
    {
        Debug.Log("TryPickUp");
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, pickupLayerMask))
        {
            GameObject target = hit.transform.root.gameObject;
            GrabClient_Rpc();

            OnHandsEvent?.Invoke(hit);
            SetupPlayerJoint(target);
        }
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void GrabClient_Rpc()
    {
        OnPickupEvent?.Invoke();
    }

    [SerializeField] private float driveSpringValue;

    void SetupPlayerJoint(GameObject target)
    {
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


    private void DropObject()
    {
        Debug.Log("Dropping object");
        Destroy(playerJoint);
        ClientDrop_Rpc();
        isHolding.Value = false;
    }
[Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void ClientDrop_Rpc()
    {
        OnDroppedEvent?.Invoke();
    }
}