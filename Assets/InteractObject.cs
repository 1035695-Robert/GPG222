using System;
using UnityEngine;


    public class InteractObject : MonoBehaviour
    {
        private FixedJoint _joint;
        private Rigidbody _rb;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _joint = GetComponent<FixedJoint>();
        }
        public void Pickup(GameObject player)
        {
            _rb.isKinematic = false;
            _joint.connectedBody = player.GetComponent<Rigidbody>();
        }public void Drop(GameObject player)
        {
            _rb.isKinematic = false;
            _joint.connectedBody = player.GetComponent<Rigidbody>();
        }
    }

