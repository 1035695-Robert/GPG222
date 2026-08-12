using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public struct CustomTransformPackage : INetworkSerializable
{
    //what do I need to network

    //y-axis rotation
    public Quaternion NetworkRotation;

    // position X and z axis 
    public Vector3 NetworkPosition;

    //Movement
    public Vector3 NetworkLinearVelocity;
    public Vector3 NetworkAngularVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetworkRotation);
        serializer.SerializeValue(ref NetworkPosition);
        serializer.SerializeValue(ref NetworkLinearVelocity);
        serializer.SerializeValue(ref NetworkAngularVelocity);
    }
}

public class NetworkMovementTransform : NetworkBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    [SerializeField] private float sendRate;
    [SerializeField] private float interpolateSpeed = 15;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        StartCoroutine(TransformUpdateLoop());
    }

    private IEnumerator TransformUpdateLoop()
    {
        var delay = new WaitForSeconds(sendRate);
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
                NetworkPosition = playerTransform.position,
                NetworkRotation = playerTransform.rotation,
                NetworkLinearVelocity = playerRigidbody.linearVelocity,
                NetworkAngularVelocity = playerRigidbody.angularVelocity
            });
            yield return new WaitForFixedUpdate();
        }
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Unreliable)]
    private void UpdatePositionPhysics_Rpc(CustomTransformPackage package)
    {
        //updates the Local Variables based on the serverVariable.
        targetRotation = package.NetworkRotation;
        targetPosition = package.NetworkPosition;
        playerRigidbody.linearVelocity = package.NetworkLinearVelocity;
        playerRigidbody.angularVelocity = package.NetworkAngularVelocity;
    }

    private void Update()
    {
        //updates the position of the Player on all clients besides the Server 
        if (!IsServer)
        {
            playerTransform.position = Vector3.Lerp(
                playerTransform.position,
                targetPosition,
                interpolateSpeed * Time.deltaTime);

            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                interpolateSpeed * Time.deltaTime);
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines(); // was originally to stop the Coroutine from Looping for other object that dont despawn
    }
}