using System;
using Unity.Netcode;
using UnityEngine;


namespace Player.hands
{
    public class HandsModel : NetworkBehaviour
    {
        private FixedJoint _handGrabJoint;

        [SerializeField] private PlayerInteractModel interactModel;
        [SerializeField] private NetworkObject player;
        [SerializeField] private Rigidbody handRigidbody;


        [SerializeField] private GameObject target;
        [SerializeField] private Vector3 targetPoint;

        public delegate void GrabHandler(NetworkObject target, Vector3 point);

        public delegate void DropHandler(NetworkObject target);

        public event GrabHandler GrabClient;

        public event DropHandler Drop;

        public void Setup()
        {
            if (!IsServer) return;
            interactModel = transform.root.GetComponent<PlayerInteractModel>();
            interactModel.isHolding.OnValueChanged += OnGrabChangeState;
            player = transform.root.GetComponent<NetworkObject>();
            interactModel.OnHandsEvent += GrabJointInformation;
        }

        private void GrabJointInformation(RaycastHit hitInfo)
        {
            target = hitInfo.transform.root.gameObject;
            targetPoint = hitInfo.point;
        }

        private void OnGrabChangeState(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                Grabbed();
            }
            else
                Dropped();
        }

        private void Grabbed()
        {
            NetworkObject grabObject = target.GetComponent<NetworkObject>();
            GrabClient?.Invoke(grabObject, targetPoint);
            handRigidbody.isKinematic = false;
            _handGrabJoint = handRigidbody.gameObject.AddComponent<FixedJoint>();
            _handGrabJoint.connectedBody = target.GetComponent<Rigidbody>();
        }

        private void Dropped()
        {
            Drop?.Invoke(player);
            Destroy(_handGrabJoint);

            handRigidbody.isKinematic = true;
        }

        #region hands Rotation
        // [Header("Hand Movement")] [SerializeField]
        // private float handInputValue;
        //
        // [SerializeField] float currentHandRotation;
        // [SerializeField] private float minHandAngle = -90;
        // [SerializeField] private float maxHandAngle = 0;
        // [SerializeField] private float rotationSpeed = 5f;
        // [SerializeField] private Rigidbody hands;

        // private void FixedUpdate()
        // {
        //     if (!IsServer) return;
        //     if (handInputValue == 0) return;
        //     HandRotation();
        // }

        // private void HandRotation()
        // {
        //     if (interactModel.isHolding.Value) return;
        //     float nextRotation = currentHandRotation + (handInputValue * rotationSpeed * 10f) * Time.deltaTime;
        //
        //     if (nextRotation >= minHandAngle && nextRotation <= 0)
        //     {
        //         currentHandRotation = nextRotation;
        //     }
        //     else if (nextRotation < minHandAngle)
        //     {
        //         currentHandRotation = minHandAngle;
        //     }
        //     else if (nextRotation > maxHandAngle)
        //     {
        //         currentHandRotation = maxHandAngle;
        //     }
        //
        //     hands.transform.localRotation = Quaternion.Euler(currentHandRotation, 0, 0);
        // }

        // public void HandRotationValue(float handAngleValue)
        // {
        //     handInputValue = handAngleValue;
        // }
        #endregion
        
        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            base.OnNetworkDespawn();
            interactModel.isHolding.OnValueChanged -= OnGrabChangeState;
            interactModel.OnHandsEvent -= GrabJointInformation;
        }
    }
}