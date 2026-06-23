using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class HandsModel : NetworkBehaviour
    {
       [SerializeField] private FixedJoint handGrabJoint;
       [SerializeField] private PlayerModel playerModel;
       

        public void OnSpawn(NetworkObject player)
        {
            playerModel = player.GetComponent<PlayerModel>();
            playerModel.OnHandsEvent += GrabJoint;
        }

       private void GrabJoint(RaycastHit hitInfo)
       {
           Debug.Log("grab");
           // GameObject target = hitInfo.transform.root.gameObject;
           // handGrabJoint = gameObject.AddComponent<FixedJoint>();
           // handGrabJoint.connectedBody = target.GetComponent<Rigidbody>();
           // transform.position = target.transform.position + new Vector3(0, 0.25f, 0);
       }

        public override void OnNetworkDespawn()
        {
            playerModel.OnHandsEvent -= GrabJoint;
        }
    }
}