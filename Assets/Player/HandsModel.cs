using System;
using Unity.Netcode;
using UnityEngine;
using Player;
namespace Player
{
    public class HandsModel : NetworkBehaviour
    {
       [SerializeField] private FixedJoint handGrabJoint;
       [SerializeField] private PlayerModel model;
       [SerializeField] private NetworkObject player;
       [SerializeField] private Rigidbody handRigidbody;

       public void Setup()
       {
           model =transform.root.GetComponent<PlayerModel>();
           player =  transform.root.GetComponent<NetworkObject>();
           model.OnHandsEvent += GrabJoint;
           model.OnDroppedEvent += DropJoint;
       }
       private void GrabJoint(RaycastHit hitInfo)
       {
           Debug.Log("grab");
           GameObject target = hitInfo.transform.root.gameObject;
           transform.position = hitInfo.point + new Vector3(0, 0.25f, 0);
           handRigidbody.isKinematic = false;
           handGrabJoint = gameObject.AddComponent<FixedJoint>();
           handGrabJoint.connectedBody = target.GetComponent<Rigidbody>();
       }

       private void DropJoint()
       {
           Destroy(handGrabJoint);
           transform.localPosition = new Vector3(0, 0, 1f);
           handRigidbody.isKinematic = true;
       }

       public override void OnNetworkDespawn()
       {
           base.OnNetworkDespawn();
       }
    }
}