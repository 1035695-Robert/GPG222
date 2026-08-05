using System.Collections;
using Unity.Netcode;
using UnityEngine;

public struct CustomTransformPackage : INetworkSerializable
{
    //what do I need to network

    //y-axis rotation
    public Quaternion networkRotation;

    // position X and z axis 
    public Vector3 networkPosition;

    //Movement
    public Vector3 networkLinearVelocity;
    public Vector3 networkAngularVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref networkRotation);
        serializer.SerializeValue(ref networkPosition);
        serializer.SerializeValue(ref networkLinearVelocity);
        serializer.SerializeValue(ref networkAngularVelocity);
    }
}

public class NetworkMovementTransform : NetworkBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        StartCoroutine(TransformUpdateLoop());
    }

    private IEnumerator TransformUpdateLoop()
    {
        while (true)
        {
            if (!IsSpawned)
            {
                //this also helps safely to prevent gameObjects that are needing this networkTransform.
                yield return null;
                continue;
                //continue: ends the loop it was just doing and continues.
            }

            UpdatePositionPhysics_Rpc(new CustomTransformPackage()
            {
                networkPosition = playerTransform.position,
                networkRotation = playerTransform.rotation,
                networkLinearVelocity = playerRigidbody.linearVelocity,
                networkAngularVelocity = playerRigidbody.angularVelocity
            });
            yield return new WaitForFixedUpdate();
        }
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void UpdatePositionPhysics_Rpc(CustomTransformPackage package)
    {
        //updates the Local Variables based on the serverVariable.
        playerTransform.rotation = package.networkRotation;
        playerTransform.position = package.networkPosition;
        playerRigidbody.linearVelocity = package.networkLinearVelocity;
        playerRigidbody.angularVelocity = package.networkAngularVelocity;
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();// was originally to stop the Coroutine from Looping for other object that dont despawn
    }
}