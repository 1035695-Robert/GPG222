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

        public event GrabHandler Grab;

        public event DropHandler Drop;

        public void Setup()
        {
            interactModel = transform.root.GetComponent<PlayerInteractModel>();
            interactModel.isHolding.OnValueChanged += OnGrabChangeState;
            player = transform.root.GetComponent<NetworkObject>();
            interactModel.OnHandsEvent += GrabJointInformation;
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
            
            SendGrab_Rpc(target, targetPoint);
            handRigidbody.isKinematic = false;
            _handGrabJoint = gameObject.AddComponent<FixedJoint>();
            _handGrabJoint.connectedBody = target.GetComponent<Rigidbody>();
        }

        private void Dropped()
        {
            Destroy(_handGrabJoint);

            SendDrop_Rpc(player);
            handRigidbody.isKinematic = true;
        }


        private void GrabJointInformation(RaycastHit hitInfo)
        {
            target = hitInfo.transform.root.gameObject;
            targetPoint = hitInfo.point;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendGrab_Rpc(NetworkObjectReference targetRef, Vector3 point)
        {
            if (targetRef.TryGet(out NetworkObject targetObject))
            {
                Grab?.Invoke(targetObject, point);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendDrop_Rpc(NetworkObjectReference targetRef)
        {
            if (targetRef.TryGet(out NetworkObject targetObject))
            {
                Drop?.Invoke(targetObject);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            interactModel.isHolding.OnValueChanged -= OnGrabChangeState;
            interactModel.OnHandsEvent -= GrabJointInformation; 
        }
    }
}