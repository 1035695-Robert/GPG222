using UnityEngine;

namespace Player
{
    public class PlayerPickup : MonoBehaviour
    {
        [SerializeField] private LayerMask pickupLayerMask;
        [SerializeField] private Transform playerTransform;

        private FixedJoint _playerPickupJoint;

        [SerializeField] private float maxDistance;

        private void OnEnable()
        {
            _playerPickupJoint = GetComponent<FixedJoint>();
            playerTransform = GetComponent<Transform>();
            PlayerInputs.OnPickup += Pickup;
        }

        private void OnDisable()
        {
            PlayerInputs.OnPickup -= Pickup;
        }

        void Pickup()
        {
            if (!_playerPickupJoint.connectedBody)
            {
                if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit raycastHit,
                        maxDistance))
                {
                    if (raycastHit.transform.CompareTag("Interactable"))
                    {
                        _playerPickupJoint.connectedBody = raycastHit.rigidbody;
                        //raycastHit.transform.rotation = playerTransform.localRotation;
                        raycastHit.rigidbody.isKinematic = false;
                    }
                }
            }
            else
            {
                _playerPickupJoint.connectedBody.isKinematic = true;
                _playerPickupJoint.connectedBody = null;
            }
        }
    }
}